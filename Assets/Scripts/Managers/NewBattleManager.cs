using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBattleManager : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Spawn Points")]
    [SerializeField] private Transform[] m_spawnPointsPlayers;
    [SerializeField] private Transform[] m_spawnPointsEnemies;

    [Header("Battle List")]
    [SerializeField] private List<UnitBase> m_unitsInBattle;
    [SerializeField] private List<UnitBase> m_turnOrder;

    [SerializeField] private CinemachineVirtualCamera m_enemyCamera;
    [SerializeField] private CinemachineVirtualCamera m_playerCamera;
    [SerializeField] private CinemachineVirtualCamera m_battleCamera;

    // --- Instance ---
    public static NewBattleManager Instance { get; private set; }

    // --- Private ---
    private int _indexTarget = 0;
    private int _indexCurrentUnit = 0;
    private int _oldTarget = 0;

    private bool _isPlayerActing = false;
    private bool _pendingBattleEnd = false;

    private BattleStatus _battleStatus;
    private BattleResult _pendingResult;
    private PlayerActionState _playerActionState;
    private EUnitTeam _selectingTeam;

    private ItemData _selectedItem;

    private Camera _battleCamera;

    // --- Proprierties ---
    public UnitBase CurrentUnit => m_turnOrder[_indexCurrentUnit];
    public UnitBase CurrentTarget => m_unitsInBattle[_indexTarget];

    // --- Events ---
    public event Action<ItemData> OnUseItem;
    public event Action<UnitBase> OnCreateUnit;

    #region Unity methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        _battleStatus = BattleStatus.Starting;
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed += SelectAttackTarget;
    }

    private void OnDisable()
    {
        _battleStatus = BattleStatus.None;
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed -= SelectAttackTarget;
    }

    private void Update()
    {
        if (_pendingBattleEnd)
        {
            _pendingBattleEnd = false;
            BattleEnd(_pendingResult);
        }

        if (!_isPlayerActing || _battleStatus != BattleStatus.Ongoing)
            return;

        switch (_playerActionState)
        {
            case PlayerActionState.ChoosingAlly:
                _selectingTeam = EUnitTeam.Player;
                ConfirmAction();
                break;
            case PlayerActionState.ChoosingEnemy:
                _selectingTeam = EUnitTeam.Enemy;
                ConfirmAction();
                break;
        }
    }
    #endregion

    #region Setup
    /// <summary>
    /// Setup all the mandatory needs for the battle to start
    /// </summary>
    public void SetupBattle(BattleSettings battleSettings, IReadOnlyList<GameObject> playersPf)
    {
        if (_battleStatus == BattleStatus.Starting)
        {
            _battleCamera = Camera.main;

            // Instantiate player and enemy prefabs
            for (int i = 0; i < playersPf.Count; i++)
            {
                GameObject go = Instantiate(playersPf[i], m_spawnPointsPlayers[i]);
                go.TryGetComponent(out PlayerInCombat p);
                p.OnPlayerDeath += HandleUnitDeath;
                OnCreateUnit.Invoke(p);
                m_unitsInBattle.Add(p);
            }

            for (int i = 0; i < battleSettings.enemies.Length; i++)
            {
                GameObject go = Instantiate(battleSettings.enemies[i], m_spawnPointsEnemies[i]);
                go.TryGetComponent(out EnemyInCombat e);
                e.SetSpeed(UnityEngine.Random.Range(10, 20));
                e.OnEnemyDeath += HandleUnitDeath;
                OnCreateUnit.Invoke(e);
                m_unitsInBattle.Add(e);
            }

            // Ording the list by the speed
            m_turnOrder = m_unitsInBattle.OrderBy(x => x.Speed).ToList();

            // Start the first turn
            if (CurrentUnit.Team == EUnitTeam.Player)
                StartPlayerTurn();
            CurrentUnit.StartTurn();

            _battleStatus = BattleStatus.Ongoing;
        }
    }
    #endregion

    #region Select target
    public void SelectAttackTarget(InputAction.CallbackContext context)
    {
        if (_battleStatus != BattleStatus.Ongoing)
            return;

        Vector2 dir = context.ReadValue<Vector2>();
        int direction = dir.x > 0.5f ? 1 : (dir.x < -0.5f ? -1 : 0);
        if (direction == 0)
            return;

        // Indexes's selected team
        List<int> indexesTeam = m_unitsInBattle.FindAllIndexes(u => u.Team == _selectingTeam);
        if (indexesTeam.Count == 0)
            return;

        int posInTeam = indexesTeam.IndexOf(_indexTarget);
        if (posInTeam == -1) posInTeam = 0;

        int nextPos = posInTeam;

        if(CurrentTarget.transform.Find("Canvas").gameObject.activeSelf)
            CurrentTarget.transform.Find("Canvas").gameObject.SetActive(false);

        do
        {
            nextPos += direction;

            if (nextPos < 0 || nextPos >= indexesTeam.Count)
            {
                nextPos -= direction;
                break;
            }
        }
        while (m_unitsInBattle[indexesTeam[nextPos]].IsDead);

        _indexTarget = indexesTeam[nextPos];
        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(true);
    }
    #endregion

    #region Buttons logic
    /// <summary>
    /// Attack the selected enemy
    /// </summary>
    public void BTNAttack()
    {
        if (_isPlayerActing && _battleStatus == BattleStatus.Ongoing)
        {
            _isPlayerActing = false;
            void HandleEndAttack()
            {
                CurrentUnit.OnEndAttack -= HandleEndAttack;
                ChangeTurn();
            }

            CurrentUnit.OnEndAttack += HandleEndAttack;
            CurrentUnit.StartAttackAnimation(CurrentTarget);
        }
    }

    /// <summary>
    /// Skip the turn of the current player
    /// </summary>
    public void BTNSkipTurn()
    {
        if (_isPlayerActing && _battleStatus == BattleStatus.Ongoing)
        {
            _isPlayerActing = false;
            ChangeTurn();
        }
    }

    public void BTNEscape()
    {
        if (_isPlayerActing && _battleStatus == BattleStatus.Ongoing)
        {
            _isPlayerActing = false;
            _battleStatus = BattleStatus.Ending;
            BattleEnd(BattleResult.Escape);
        }
    }

    /// <summary>
    /// Use item logic
    /// </summary>
    /// <param name="item"></param>
    /// 
    public void BTNUseItem(ItemData item)
    {
        if (!_isPlayerActing || _battleStatus != BattleStatus.Ongoing)
            return;

        _selectedItem = item;
        if (item.Item.type == ItemType.damage)
        {
            m_battleCamera.Priority = 0;
            m_enemyCamera.Priority = 10;
            _playerActionState = PlayerActionState.ChoosingEnemy;
        }
        else
        {
            m_battleCamera.Priority = 0;
            m_playerCamera.Priority = 10;
            _playerActionState = PlayerActionState.ChoosingAlly;
        }
    }

    /// <summary>
    /// Use ability logic.
    /// The ability can't be use if is not completely charge
    /// </summary>
    /// <param name="ability"></param>
    public void BTNUseAbility(AbilityData ability)
    {
        if (_isPlayerActing && ability.ChargeCounter == ability.Ability.maxCharge && _battleStatus == BattleStatus.Ongoing)
        {
            _isPlayerActing = false;
            List<UnitBase> enemies = new() { CurrentTarget };
            enemies.AddRange(m_unitsInBattle.FindAll(e => e.Team == EUnitTeam.Enemy && e != CurrentTarget));
            ability.UseAbility(enemies.ToArray());
            ChangeTurn();
        }
    }
    #endregion

    private void ConfirmAction()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _playerActionState = PlayerActionState.Acting;
            StartCoroutine(ExecuteItemUse());
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            _playerActionState = PlayerActionState.Idle;
            CancelItemAction();
        }
    }
    private IEnumerator ExecuteItemUse()
    {
        // Esegui effetto (puoi mettere animazioni, delay, ecc.)
        _selectedItem.UseItem(CurrentTarget);
        OnUseItem?.Invoke(_selectedItem);

        yield return new WaitForSeconds(0.5f);

        // Ripristina camera e resetta stato
        _playerActionState = PlayerActionState.Idle;
        _isPlayerActing = false;

        if (m_playerCamera.Priority > 0)
        {
            m_battleCamera.Priority = 10;
            m_playerCamera.Priority = 0;
        }
        else if (m_enemyCamera.Priority > 0)
        {
            m_battleCamera.Priority = 10;
            m_enemyCamera.Priority = 0;
        }

            yield return new WaitForSeconds(0.5f);

        ChangeTurn();
    }
    private void CancelItemAction()
    {
        _playerActionState = PlayerActionState.Idle;
        _selectedItem = null;
        if (m_playerCamera.Priority > 0)
        {
            m_battleCamera.Priority = 10;
            m_playerCamera.Priority = 0;
        }
        else if (m_enemyCamera.Priority > 0)
        {
            m_battleCamera.Priority = 10;
            m_enemyCamera.Priority = 0;
        }
        Debug.Log("Azione annullata");
    }

    #region Turn management
    private void ChangeTurn()
    {
        if (_battleStatus == BattleStatus.Ongoing)
        {
            CurrentUnit.EndTurn();
            if (CurrentUnit.Team == EUnitTeam.Player)
                EndPlayerTurn();

            do
            {
                _indexCurrentUnit += 1;

                if (_indexCurrentUnit >= m_unitsInBattle.Count) _indexCurrentUnit = 0;
                else if (_indexCurrentUnit < 0) _indexCurrentUnit = m_unitsInBattle.Count - 1;

            } while (CurrentUnit.IsDead);

            if (CurrentUnit.Team == EUnitTeam.Player)
                StartPlayerTurn();
            else
                EnemyTurn();

            CurrentUnit.StartTurn();
        }
    }
    private void StartPlayerTurn()
    {
        _isPlayerActing = true;

        if (m_unitsInBattle[_oldTarget].Team == EUnitTeam.Enemy && !m_unitsInBattle[_oldTarget].IsDead)
        {
            _indexTarget = _oldTarget;
        }
        else
        {
            _indexTarget = m_unitsInBattle.FindIndex(e => e.Team == EUnitTeam.Enemy && !e.IsDead);
        }
        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(true);

    }
    private void EndPlayerTurn()
    {
        _isPlayerActing = false;
        _selectingTeam = EUnitTeam.Enemy;

        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(false);

        // Check if the current target is alive and is an enemy.
        // If yes, store the index for the next turn
        // else search the first alive enemy index
        if (CurrentTarget.Team == EUnitTeam.Enemy && !CurrentTarget.IsDead)
            _oldTarget = _indexTarget;
        else
            _oldTarget = m_unitsInBattle.FindIndex(e => !e.IsDead);
    }
    private void EnemyTurn()
    {
        void HandleEndAttack()
        {
            CurrentUnit.OnEndAttack -= HandleEndAttack;
            ChangeTurn();
        }

        CurrentUnit.OnEndAttack += HandleEndAttack;

        UnitBase target = m_unitsInBattle.Find(p => p.Team == EUnitTeam.Player);
        if (target != null)
            CurrentUnit.StartAttackAnimation(target);
    }
    #endregion

    #region EndBattle and death
    private BattleResult CheckBattleResult()
    {
        bool isPlayersAlive = m_unitsInBattle.Exists(p => !p.IsDead);
        bool isEnemiesAlive = m_unitsInBattle.Exists(e => !e.IsDead);

        if (!isPlayersAlive) return BattleResult.Enemy;
        if (!isEnemiesAlive) return BattleResult.Player;

        return BattleResult.None;
    }
    private void BattleEnd(BattleResult winner)
    {
        if (winner != BattleResult.None && _battleStatus == BattleStatus.Ongoing) return;

        _battleStatus = BattleStatus.Ending;
        Debug.Log($"Winner {winner}");
        foreach (UnitBase unit in m_unitsInBattle)
            Destroy(unit.gameObject);
        m_unitsInBattle.Clear();
        GameEvents.BattleEnd(winner);
    }

    private void HandleUnitDeath()
    {
        if (m_unitsInBattle.Find(u => u.Health <= 0 && !u.IsDead) is UnitBase unit)
        {
            if (CurrentUnit == unit)
                ChangeTurn();
            unit.SetDead();
            unit.gameObject.SetActive(false);

            BattleResult winner = CheckBattleResult();
            if (winner != BattleResult.None)
            {
                _pendingResult = winner;
                _pendingBattleEnd = true;
            }
        }
    }
    #endregion

    public IReadOnlyList<UnitBase> GetUnits() => m_unitsInBattle;
}