using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBattleManager : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPointsPlayers;
    [SerializeField] private Transform[] _spawnPointsEnemies;

    [Header("Battle List")]
    [SerializeField] private List<UnitBase> _unitsInBattle;

    // --- Instance ---
    public static NewBattleManager Instance { get; private set; }

    // --- Private ---
    private int _indexTarget = 0;
    private int _indexCurrentUnit = 0;
    private int _oldTarget = 0;

    private bool _isPlayerActing = false;

    private UnitBase _currentUnit;
    private Camera _battleCamera;

    // --- Events ---
    public event Action<ItemData> OnUseItem;
    public event Action<AbilityData> OnUseAbility;

    #region Unity Methods
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
            GameObject go = Instantiate(playersPf[i], _spawnPointsPlayers[i]);
            go.TryGetComponent(out PlayerInCombat p);
            _unitsInBattle.Add(p);
        }

        for (int i = 0; i < battleSettings.enemies.Length; i++)
        {
            GameObject go = Instantiate(battleSettings.enemies[i], _spawnPointsEnemies[i]);
            go.TryGetComponent(out EnemyInCombat e);
            e.SetSpeed(UnityEngine.Random.Range(10, 20));
            _unitsInBattle.Add(e);
        }

        // Ording the list by the speed
        _unitsInBattle = _unitsInBattle.OrderBy(x => x.Speed).ToList();

        // Start the first turn
        _currentUnit = _unitsInBattle[0];
        _currentUnit.StartTurn();
    }
    #endregion

    private void SelectAttackTarget(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        int direction;

        if (dir.x > 0.5f) direction = 1;        // Right
        else if (dir.x < -0.5f) direction = -1; // Left
        else return;

        // Select the index of the wanted enemy, repeat until the target is an enemy
        do
        {
            _indexTarget += direction;

            if (_indexTarget >= _unitsInBattle.Count) _indexTarget = 0;
            else if (_indexTarget < 0) _indexTarget = _unitsInBattle.Count - 1;

        } while (_unitsInBattle[_indexTarget].Team == EUnitTeam.player);

        Debug.Log($"Target selected - {_indexTarget} {_unitsInBattle[_indexTarget].Team}");
    }

    private void ChangeTurn()
    {
        _currentUnit.EndTurn();
        if (_currentUnit.Team == EUnitTeam.player)
            EndPlayerTurn();

        do _indexCurrentUnit += 1;
        while (_unitsInBattle[_indexCurrentUnit].Health <= 0);

        _currentUnit = _unitsInBattle[_indexCurrentUnit];

        if (_currentUnit.Team == EUnitTeam.player)
            StartPlayerTurn();
        _currentUnit.StartTurn();
    }

    #region Buttons Logic
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
                _currentUnit.OnEndAttack -= HandleEndAttack;
                EndPlayerTurn();
            }

            _currentUnit.OnEndAttack += HandleEndAttack;
            _currentUnit.StartAttackAnimation(_unitsInBattle[_indexTarget]);
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
            EndPlayerTurn();
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
            item.UseItem(_unitsInBattle[_indexTarget]);
            OnUseItem?.Invoke(item);
            EndPlayerTurn();
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
            List<UnitBase> enemies = new() { _unitsInBattle[_indexTarget] };
            enemies.AddRange(_unitsInBattle.FindAll(e => e.Team == EUnitTeam.enemy && e != _unitsInBattle[_indexTarget]));
            ability.UseAbility(enemies.ToArray());
            OnUseAbility?.Invoke(ability);
            ChangeTurn();
        }
    }
    #endregion

    #region Turn management
    private void StartPlayerTurn()
    {
        _isPlayerActing = true;
        _indexTarget = _oldTarget;
    }

    private void EndPlayerTurn()
    {
        _isPlayerActing = false;

        // Check if the current target is alive and is an enemy.
        // If yes, store the index for the next turn
        // else search the first alive enemy index
        if (_unitsInBattle[_indexTarget].Health > 0 && _unitsInBattle[_indexTarget].Team == EUnitTeam.enemy)
            _oldTarget = _indexTarget;
        else
            _oldTarget = _unitsInBattle.FindIndex(e => e.Team == EUnitTeam.enemy && e.Health > 0);
    }
    #endregion
}