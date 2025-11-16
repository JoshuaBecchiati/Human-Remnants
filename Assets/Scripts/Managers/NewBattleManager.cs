using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;

public class NewBattleManager : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Spawn Points")]
    [SerializeField] private Transform m_playerSide;
    [SerializeField] private Transform m_enemySide;
    [SerializeField] private float m_space = 5f;

    [Header("Battle List")]
    [SerializeField] private List<UnitBase> m_unitsInBattle;
    [SerializeField] private List<UnitBase> m_turnOrder;

    [Header("Dependency")]
    [SerializeField] private BattleCameraManager m_cameraController;
    [SerializeField] private ActionSelector m_actionSelector;
    [SerializeField] private UIBattleManager m_UIManager;
    [SerializeField] private CinematicManager m_cinematicManager;
    [SerializeField] private CinematicBattleManager m_cinematicBattleManager;

    [Header("Cinematic")]
    [SerializeField] private SignalReceiver _signalReceiver;

    // --- Instance ---
    public static NewBattleManager Instance { get; private set; }

    // --- Private ---
    private int _indexTarget;
    private int _indexCurrentUnit;
    private int _oldTarget;

    private bool _isFirstTurn;

    private BattleStatus _battleStatus;
    private BattleResult _pendingResult;
    private EUnitTeam _selectingTeam;

    private ItemData _selectedItem;
    private AbilityData _selectedAbility;

    // --- Proprierties ---
    public UnitBase CurrentUnit => m_turnOrder[_indexCurrentUnit];
    public UnitBase CurrentTarget => m_unitsInBattle[_indexTarget];

    // --- Events ---
    public event Action<ItemData> OnUseItem;
    public event Action<UnitBase> OnCreateUnit;
    public event Func<UnitBase, IEnumerator> OnStartAttack;
    public event Action<List<UnitBase>, SignalReceiver> OnStartBattleCinematic;

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
        _pendingResult = BattleResult.None;
        _selectingTeam = EUnitTeam.Enemy;
        _isFirstTurn = true;
        _indexTarget = 0;
        _indexCurrentUnit = 0;
        _oldTarget = 0;

        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed += SelectAttackTarget;
        if (MainGameManager.Instance != null)
            MainGameManager.Instance.OnSetupBattle += StartCinematicBattle;
    }

    private void OnDisable()
    {
        _battleStatus = BattleStatus.None;
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed -= SelectAttackTarget;
        if (MainGameManager.Instance != null)
            MainGameManager.Instance.OnSetupBattle -= StartCinematicBattle;
    }

    private void Update()
    {
        switch(_battleStatus)
        {
            case BattleStatus.ChangingTurn:
                ChangeTurn();
                break;
            case BattleStatus.CheckingEnd:
                CheckingBattleEnd();
            break;
            case BattleStatus.Ending:
                BattleEnd();
            break;
        }
    }
    #endregion

    #region Setup
    /// <summary>
    /// Setup all the mandatory needs for the battle to start
    /// </summary>
    public void StartCinematicBattle(BattleSettings battleSettings, IReadOnlyList<GameObject> playersPf)
    {
        if (_battleStatus != BattleStatus.Starting)
            return;


        InstantiatePrefab(playersPf.ToList(), m_playerSide);
        OnStartBattleCinematic.Invoke(m_unitsInBattle, _signalReceiver);

        InstantiatePrefab(battleSettings.enemies.ToList(), m_enemySide);
    }

    public void SetupBattle()
    {
        if (_battleStatus != BattleStatus.Starting)
            return;

        foreach (UnitBase u in m_unitsInBattle)
            if (u.Team == EUnitTeam.Player)
            {
                u.TryGetComponent(out AnimationTimeLine atl);
                atl.CombatController();
            }

        m_cameraController.BattleCamera();

        // Ording the list by the speed
        m_turnOrder = m_unitsInBattle.OrderBy(x => x.Speed).ToList();

        _battleStatus = BattleStatus.ChangingTurn;
    }

    public void InstantiatePrefab(List<GameObject> prefabs, Transform transformSide)
    {
        float totalWidth = m_space * (prefabs.Count - 1) / 2f;

        for (int i = 0; i < prefabs.Count; i++)
        {
            Vector3 spawnPos = transformSide.position + new Vector3((m_space * i) - totalWidth, 0f, 0f);

            GameObject go = Instantiate(prefabs[i], transformSide);
            go.transform.position = spawnPos;
            go.TryGetComponent(out UnitBase u);
            u.OnDeath += HandleUnitDeath;
            OnCreateUnit.Invoke(u);
            m_unitsInBattle.Add(u);

            if (u.Team == EUnitTeam.Player)
                go.transform.Find("Model").position += new Vector3(0f, 0f, -3f);
        }
    }
    #endregion

    #region Select target
    public void SelectAttackTarget(InputAction.CallbackContext context)
    {
        // Return if the battle isn't ongoing
        if (_battleStatus != BattleStatus.PlayerTurn)
            return;

        // Get directional value for select the target
        Vector2 dir = context.ReadValue<Vector2>();
        int direction = dir.x > 0.5f ? 1 : (dir.x < -0.5f ? -1 : 0);
        if (direction == 0)
            return;

        // Indexes's selected team
        List<int> indexesTeam = m_unitsInBattle.FindAllIndexes(u => u.Team == _selectingTeam && !u.IsDead);
        if (indexesTeam.Count == 0)
            return;

        int posInTeam = indexesTeam.IndexOf(_indexTarget);
        if (posInTeam == -1) posInTeam = 0;

        int nextPos = posInTeam;

        m_UIManager.SetOffInfoBar(CurrentTarget);

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
        m_UIManager.SetOnInfoBar(CurrentTarget);
    }
    private void SelectTarget(UnitBase newTarget)
    {
        if (_battleStatus != BattleStatus.PlayerTurn)
            return;

        // Disattiva il vecchio target
        m_UIManager.SetOffInfoBar(CurrentTarget);

        // Aggiorna l'indice
        _oldTarget = _indexTarget;
        _indexTarget = m_unitsInBattle.IndexOf(newTarget);

        // Attiva il canvas del nuovo target
        m_UIManager.SetOnInfoBar(CurrentTarget);
    }
    #endregion

    #region Buttons logic
    /// <summary>
    /// Attack the selected enemy
    /// </summary>
    public void BTNAttack()
    {
        if (_battleStatus != BattleStatus.PlayerTurn)
            return;

        m_cameraController.EnemyCamera();

        m_actionSelector.StartConfirmAction(
            onConfirm: () => StartCoroutine(ExecuteAttack()),
            onCancel: CancelAttack
        );
    }

    /// <summary>
    /// Skip the turn of the current player
    /// </summary>
    public void BTNSkipTurn()
    {
        if (_battleStatus != BattleStatus.PlayerTurn)
            return;

        _battleStatus = BattleStatus.ChangingTurn;
    }

    public void BTNEscape()
    {
        if (_battleStatus == BattleStatus.PlayerTurn)
            _battleStatus = BattleStatus.Ending;
    }

    /// <summary>
    /// Use item logic
    /// </summary>
    /// <param name="item"></param>
    /// 
    public void BTNUseItem(ItemData item)
    {
        if (_battleStatus != BattleStatus.PlayerTurn)
            return;

        _selectedItem = item;
        if (item.Item.type == ItemType.damage)
        {
            _selectingTeam = EUnitTeam.Enemy;
            m_cameraController.EnemyCamera();
        }
        else if (item.Item.type == ItemType.heal)
        {
            _selectingTeam = EUnitTeam.Player;
            m_cameraController.PlayerCamera();
            SelectTarget(CurrentUnit);
        }

        m_actionSelector.StartConfirmAction
        (
            onConfirm: () => StartCoroutine(ExecuteItem()),
            onCancel: CancelItem
        );
    }

    /// <summary>
    /// Use ability logic.
    /// The ability can't be use if is not completely charge
    /// </summary>
    /// <param name="ability"></param>
    public void BTNUseAbility(AbilityData ability)
    {
        if (ability.ChargeCounter != ability.Ability.maxCharge && _battleStatus != BattleStatus.PlayerTurn)
            return;

        m_cameraController.EnemyCamera();

        _selectedAbility = ability;

        m_actionSelector.StartConfirmAction
        (
            onConfirm: () => StartCoroutine(ExecuteAbility()),
            onCancel: CancelAbility
        );
    }
    #endregion

    #region Action Execution
    private IEnumerator ExecuteAttack()
    {
        _battleStatus = BattleStatus.Executing;
        m_cameraController.BattleCamera();

        yield return new WaitForSeconds(1.5f);

        CurrentUnit.SetTarget(CurrentTarget);

        // Avvia la coroutine dell'attacco
        if (OnStartAttack != null)
        {
            yield return StartCoroutine(OnStartAttack.Invoke(CurrentUnit));
        }

        _battleStatus = BattleStatus.CheckingEnd;
    }
    private void CancelAttack()
    {
        m_cameraController.BattleCamera();
    }
    private IEnumerator ExecuteItem()
    {
        _battleStatus = BattleStatus.Executing;
        _selectedItem.UseItem(CurrentTarget);
        OnUseItem?.Invoke(_selectedItem);

        yield return new WaitForSeconds(0.5f);

        m_cameraController.BattleCamera();

        yield return new WaitForSeconds(1.5f);

        _selectedAbility = null;
        _battleStatus = BattleStatus.ChangingTurn;
    }
    private void CancelItem()
    {
        _selectingTeam = EUnitTeam.Enemy;
        _selectedItem = null;
        SelectTarget(m_unitsInBattle[_oldTarget]);
        m_cameraController.BattleCamera();
        Debug.Log("Azione annullata");
    }
    private IEnumerator ExecuteAbility()
    {
        _battleStatus = BattleStatus.Executing;

        List<UnitBase> enemies = new() { CurrentTarget };
        enemies.AddRange(m_unitsInBattle.FindAll(e => e.Team == EUnitTeam.Enemy && e != CurrentTarget));

        _selectedAbility.UseAbility(enemies.ToArray());

        yield return new WaitForSeconds(1.5f);

        m_cameraController.BattleCamera();

        yield return new WaitForSeconds(1.5f);

        _selectedAbility = null;
        _battleStatus = BattleStatus.ChangingTurn;
    }
    private void CancelAbility()
    {
        _selectingTeam = EUnitTeam.Enemy;
        _selectedAbility = null;
        m_cameraController.BattleCamera();
        Debug.Log("Azione annullata");
    }
    #endregion

    #region Turn management
    private void ChangeTurn()
    {
        // Blocca update se stiamo eseguendo un attacco o un item
        if (_battleStatus == BattleStatus.Executing)
            return;

        // Fine turno dell’unità precedente
        if (!_isFirstTurn)
        {
            CurrentUnit.EndTurn();
            if (CurrentUnit.Team == EUnitTeam.Player)
                EndPlayerTurn();

            // Avanza all’unità successiva
            do
            {
                _indexCurrentUnit++;
                if (_indexCurrentUnit >= m_turnOrder.Count) _indexCurrentUnit = 0;
            } while (CurrentUnit.IsDead);
        }
        else
        {
            _isFirstTurn = false;
        }

        // Inizio turno unità corrente
        CurrentUnit.StartTurn();

        if (CurrentUnit.Team == EUnitTeam.Player)
        {
            StartPlayerTurn(); // Manteniamo la funzione esistente
        }
        else
        {
            // Nemico: esegui l’attacco in coroutine
            _battleStatus = BattleStatus.Executing;
            StartCoroutine(StartEnemyTurn());
        }
    }

    private void StartPlayerTurn()
    {
        _battleStatus = BattleStatus.TurnTransition;

        if (m_unitsInBattle[_oldTarget].Team == EUnitTeam.Enemy && !m_unitsInBattle[_oldTarget].IsDead)
        {
            _indexTarget = _oldTarget;
            Debug.Log("[NewBattleManager] Assegnato vecchio target");
        }
        else
        {
            _indexTarget = m_unitsInBattle.FindIndex(e => e.Team == EUnitTeam.Enemy && !e.IsDead);
            Debug.Log("[NewBattleManager] Assegnato nuovo target");
        }


        m_UIManager.SetOnInfoBar(CurrentTarget);

        _battleStatus = BattleStatus.PlayerTurn;
    }
    private void EndPlayerTurn()
    {
        _selectingTeam = EUnitTeam.Enemy;

        m_UIManager.SetOffInfoBar(CurrentTarget);

        // Check if the current target is alive and is an enemy.
        // If yes, store the index for the next turn
        // else search the first alive enemy index
        if (CurrentTarget.Team == EUnitTeam.Enemy && !CurrentTarget.IsDead)
            _oldTarget = _indexTarget;
        else
            _oldTarget = m_unitsInBattle.FindIndex(e => !e.IsDead);
    }
    private IEnumerator StartEnemyTurn()
    {
        UnitBase playerTarget = m_unitsInBattle.Find(u => u.Team == EUnitTeam.Player && !u.IsDead);
        if (playerTarget != null)
            CurrentUnit.SetTarget(playerTarget);

        if (OnStartAttack != null)
            yield return StartCoroutine(OnStartAttack.Invoke(CurrentUnit));

        CurrentUnit.EndTurn();

        _battleStatus = BattleStatus.CheckingEnd;
    }
    #endregion

    #region End battle and death
    private void CheckingBattleEnd()
    {
        if (_battleStatus != BattleStatus.CheckingEnd)
            return;

        bool isPlayersAlive = m_unitsInBattle.Exists(p => !p.IsDead && p.Team == EUnitTeam.Player);
        bool isEnemiesAlive = m_unitsInBattle.Exists(e => !e.IsDead && e.Team == EUnitTeam.Enemy);

        if (!isPlayersAlive)
            _pendingResult = BattleResult.Enemy;
        else if (!isEnemiesAlive)
            _pendingResult = BattleResult.Player;
        else
        {
            _battleStatus = BattleStatus.ChangingTurn;
            return;
        }
        _battleStatus = BattleStatus.Ending;
    }
    private void BattleEnd()
    {
        if (_battleStatus != BattleStatus.Ending)
            return;

        Debug.Log($"Winner {_pendingResult}");

        foreach (Transform child in m_playerSide.Cast<Transform>().ToArray())
            Destroy(child.gameObject);

        foreach (Transform child in m_enemySide.Cast<Transform>().ToArray())
            Destroy(child.gameObject);

        m_unitsInBattle.Clear();
        m_turnOrder.Clear();
        GameEvents.BattleEnd(_pendingResult);
    }
    private void HandleUnitDeath(UnitBase unit)
    {
        unit.SetDead();
        unit.gameObject.SetActive(false);

        if(CurrentUnit == unit)
            _battleStatus = BattleStatus.CheckingEnd;
    }
    #endregion

    public IReadOnlyList<UnitBase> GetUnits()
    {
        return m_unitsInBattle;
    }
}