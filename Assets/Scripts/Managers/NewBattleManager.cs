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
            m_unitsInBattle.Add(p);
        }

        for (int i = 0; i < battleSettings.enemies.Length; i++)
        {
            GameObject go = Instantiate(battleSettings.enemies[i], m_spawnPointsEnemies[i]);
            go.TryGetComponent(out EnemyInCombat e);
            e.SetSpeed(UnityEngine.Random.Range(10, 20));
            e.OnEnemyDeath += HandleUnitDeath;
            m_unitsInBattle.Add(e);
        }

        // Ording the list by the speed
        m_turnOrder = m_unitsInBattle.OrderBy(x => x.Speed).ToList();

        // Start the first turn
        if (CurrentUnit.Team == EUnitTeam.player)
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
            while (m_unitsInBattle[_indexTarget ].Health <= 0);
        }

        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(true);
        Debug.Log($"Target selected - {_indexTarget} {CurrentTarget.Team}");
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
            GameEvents.BattleEnd();
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
            enemies.AddRange(m_unitsInBattle.FindAll(e => e.Team == EUnitTeam.enemy && e != CurrentTarget));
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
        if (CurrentUnit.Team == EUnitTeam.player)
            EndPlayerTurn();

        do
        {
            _indexCurrentUnit += 1;

            if (_indexCurrentUnit >= m_unitsInBattle.Count) _indexCurrentUnit = 0;
            else if (_indexCurrentUnit < 0) _indexCurrentUnit = m_unitsInBattle.Count - 1;

        } while (CurrentUnit.gameObject.activeSelf == false);

        if (CurrentUnit.Team == EUnitTeam.player)
            StartPlayerTurn();
        else
            EnemyTurn();

        CurrentUnit.StartTurn();
    }
    private void StartPlayerTurn()
    {
        _isPlayerActing = true;

        if (m_unitsInBattle[_oldTarget].Team == EUnitTeam.enemy && m_unitsInBattle[_oldTarget].gameObject.activeSelf == true)
        {
            Debug.Log($"OLDTARGET {_oldTarget}");
            _indexTarget = _oldTarget;
        }
        else
            _indexTarget = m_unitsInBattle.FindIndex(e => e.Team == EUnitTeam.enemy && e.gameObject.activeSelf == true);

        m_unitsInBattle[_indexTarget].transform.Find("Canvas").gameObject.SetActive(true);

    }

    private void EndPlayerTurn()
    {
        _isPlayerActing = false;

        CurrentTarget.transform.Find("Canvas").gameObject.SetActive(false);

        // Check if the current target is alive and is an enemy.
        // If yes, store the index for the next turn
        // else search the first alive enemy index
        if (CurrentTarget.Team == EUnitTeam.enemy && CurrentTarget.gameObject.activeSelf == true)
            _oldTarget = _indexTarget;
        else
            _oldTarget = m_unitsInBattle.FindIndex(e => e.gameObject.activeSelf == true);
    }

    private void EnemyTurn()
    {
        void HandleEndAttack()
        {
            CurrentUnit.OnEndAttack -= HandleEndAttack;
            ChangeTurn();
        }

        CurrentUnit.OnEndAttack += HandleEndAttack;

        UnitBase target = m_unitsInBattle.Find(p => p.Team == EUnitTeam.player);
        if (target != null)
            CurrentUnit.StartAttackAnimation(target);
    }
    #endregion

    #region Victory and death


    private void HandleUnitDeath()
    {
        if (m_unitsInBattle.Find(u => u.Health <= 0) is UnitBase unit)
        {
            if (CurrentUnit == unit)
                ChangeTurn();
            unit.gameObject.SetActive(false);
        }
    }
    #endregion
}