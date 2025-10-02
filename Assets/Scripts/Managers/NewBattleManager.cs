using System;
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

    // --- Instance ---
    public static NewBattleManager Instance { get; private set; }

    // --- Private ---
    private int _indexTarget = 0;
    private int _indexCurrentUnit = 0;
    private int _oldTarget = 0;

    private bool _isPlayerActing = false;

    private Camera _battleCamera;

    // --- Proprierties ---
    public UnitBase CurrentUnit => m_turnOrder[_indexCurrentUnit];
    public UnitBase CurrentTarget => m_unitsInBattle[_indexTarget];

    // --- Events ---
    public event Action<ItemData> OnUseItem;
    public event Action<AbilityData> OnUseAbility;
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
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed += SelectAttackTarget;
    }

    private void OnDisable()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed -= SelectAttackTarget;
    }
    #endregion

    #region Setup
    /// <summary>
    /// Setup all the mandatory needs for the battle to start
    /// </summary>
    public void SetupBattle(BattleSettings battleSettings, IReadOnlyList<GameObject> playersPf)
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
    }
    #endregion

    #region Select target
    private void SelectAttackTarget(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        int direction;

        if (dir.x > 0.5f) direction = 1;        // Right
        else if (dir.x < -0.5f) direction = -1; // Left
        else return;

        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(false);

        // Select the index of the wanted enemy, repeat until the target is an enemy
        if (_indexTarget + direction > 0 && _indexTarget + direction < m_unitsInBattle.Count)
        {
            do _indexTarget += direction;
            while (CurrentTarget.IsDead);
        }

        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(true);
    }
    #endregion

    #region Buttons logic
    /// <summary>
    /// Attack the selected enemy
    /// </summary>
    public void BTNAttack()
    {
        if (_isPlayerActing)
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
        if (_isPlayerActing)
        {
            _isPlayerActing = false;
            ChangeTurn();
        }
    }

    public void BTNEscape()
    {
        if (_isPlayerActing)
        {
            _isPlayerActing = false;
            BattleEnd(EUnitTeam.Escape);
        }
    }

    /// <summary>
    /// Use item logic
    /// </summary>
    /// <param name="item"></param>
    public void BTNUseItem(ItemData item)
    {
        if (_isPlayerActing)
        {
            item.UseItem(CurrentTarget);
            OnUseItem?.Invoke(item);
            ChangeTurn();
        }
    }

    /// <summary>
    /// Use ability logic.
    /// The ability can't be use if is not completely charge
    /// </summary>
    /// <param name="ability"></param>
    public void BTNUseAbility(AbilityData ability)
    {
        if (_isPlayerActing && ability.ChargeCounter == ability.Ability.maxCharge)
        {
            _isPlayerActing = false;
            List<UnitBase> enemies = new() { CurrentTarget };
            enemies.AddRange(m_unitsInBattle.FindAll(e => e.Team == EUnitTeam.Enemy && e != CurrentTarget));
            ability.UseAbility(enemies.ToArray());
            OnUseAbility?.Invoke(ability);
            ChangeTurn();
        }
    }
    #endregion

    #region Turn management
    private void ChangeTurn()
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
    private EUnitTeam CheckBattleResult()
    {
        bool isPlayersAlive = m_unitsInBattle.Any(p => !p.IsDead);
        bool isEnemiesAlive = m_unitsInBattle.Any(e => !e.IsDead);

        if (isPlayersAlive) return EUnitTeam.Player;
        if (isEnemiesAlive) return EUnitTeam.Enemy;

        return EUnitTeam.None;

    }
    private void BattleEnd(EUnitTeam escape)
    {
        GameEvents.BattleEnd(escape);
    }
    private void BattleEnd()
    {
        EUnitTeam winnerTeam = CheckBattleResult();
        if (winnerTeam != EUnitTeam.None)
            GameEvents.BattleEnd(winnerTeam);
    }

    private void HandleUnitDeath()
    {
        if (m_unitsInBattle.Find(u => u.Health <= 0) is UnitBase unit)
        {
            if (CurrentUnit == unit)
                ChangeTurn();
            unit.SetDead();
        }

        // Check se la battaglia è finita
        BattleEnd();
    }
    #endregion

    public IReadOnlyList<UnitBase> GetUnits() => m_unitsInBattle;
}