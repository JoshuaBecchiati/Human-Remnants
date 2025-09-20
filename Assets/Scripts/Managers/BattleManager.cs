using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-5)]
public class BattleManager : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPointsPlayers;
    [SerializeField] private Transform[] _spawnPointsEnemies;

    [Header("Prefabs")]
    [SerializeField] private GameObject[] _playersPF;
    [SerializeField] private GameObject[] _enemiesPF;

    // --- Runtime State ---
    [SerializeField] private List<UnitBase> _unitsInBattle = new List<UnitBase>();
    private List<GameObject> _playersDeathPF = new List<GameObject>();

    private Queue<UnitBase> turnQueue = new Queue<UnitBase>();
    private UnitBase CurrentUnit;
    private int _selectedTarget = 0;
    private bool _isFighting = true;
    private bool _isPlayerActing;
    private Camera _camera;

    // --- Static ---
    public static BattleManager Instance { get; private set; }

    // --- Events ---
    public event Action<UnitBase> OnCreateUnit;
    public event Action OnCloseBattle;
    public event Action<UnitBase, ItemData> OnUseItem;

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
            PlayerInputSingleton.Instance.Actions["Combat"].performed += SelectTarget;

        _isFighting = true;
        _camera = Camera.main;
        SetupBattle();
    }
    private void OnDisable()
    {
        if(PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed -= SelectTarget;

        _isFighting = false;
        _camera = null;

        if (_unitsInBattle != null)
        {
            foreach (var unit in _unitsInBattle)
                if (unit != null)
                    Destroy(unit.gameObject);

            _unitsInBattle.Clear();
        }

        turnQueue.Clear();
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Combat"].performed -= SelectTarget;
    }

    void Update()
    {
        if (!_isFighting) return;
        Victory();
    }
    #endregion

    #region Input actions management
    /// <summary>
    /// Change the selected enemy
    /// </summary>
    /// <param name="context"></param>
    private void SelectTarget(InputAction.CallbackContext context)
    {
        var dir = context.ReadValue<Vector2>();

        if (!_isFighting || CurrentUnit == null) return;

        int direction = 0;

        if (dir.x > 0.5f) direction = 1;        // Destra
        else if (dir.x < -0.5f) direction = -1; // Sinistra
        else return;

        for (int i = 0; i < _unitsInBattle.Count; i++)
        {
            _selectedTarget = (_selectedTarget + direction + _unitsInBattle.Count) % _unitsInBattle.Count;

            if (_unitsInBattle[_selectedTarget].Team == EUnitTeam.enemy)
            {
                Debug.Log($"Target selected - {_selectedTarget}\n" +
                          $"---[Statistics]---\n" +
                          $"Current health: {_unitsInBattle[_selectedTarget].Health}\n");
                return;
            }
        }

        _selectedTarget = -1;
        Debug.Log("Nessun nemico disponibile da selezionare");
    }
    #endregion

    #region Turns Management
    /// <summary>
    /// Check if CurrentUnit is a player and set the _selectedTarget to an enemy
    /// </summary>
    private void PlayerTurn()
    {
        _isPlayerActing = true;
        if (CurrentUnit.Team == EUnitTeam.player)
        {
            UnitBase target = _unitsInBattle.Find(p => p.Team == EUnitTeam.enemy);
            if (target != null)
                _selectedTarget = _unitsInBattle.FindIndex(e => e.Team == EUnitTeam.enemy);
        }
    }
    /// <summary>
    /// Check if CurrentUnit is an enemy and manage its actions
    /// </summary>
    private void EnemyTurn()
    {
        if (CurrentUnit.Team == EUnitTeam.enemy)
        {
            void HandleEndAttack()
            {
                CurrentUnit.OnEndAttack -= HandleEndAttack;
                NextTurn();
            }

            CurrentUnit.OnEndAttack += HandleEndAttack;

            UnitBase target = _unitsInBattle.Find(p => p.Team == EUnitTeam.player);
            if (target != null)
                CurrentUnit.StartAttackAnimation(target);

        }
    }
    /// <summary>
    /// End the turn of the previous unit and start turn of the following unit
    /// </summary>
    public void NextTurn()
    {
        if (turnQueue.Count == 0) return;

        CurrentUnit?.EndTurn();

        if(CurrentUnit == null || CurrentUnit.AccumulatedSpeed < CurrentUnit.SpeedNextTurn)
        {
            do
            {
                if (turnQueue.Count == 0) return;
                CurrentUnit = turnQueue.Dequeue();
            } while (CurrentUnit == null || CurrentUnit.Health <= 0);
        }
        else
            CurrentUnit.ResetAccumulatedSpeed();

        CurrentUnit.StartTurn();

        if (CurrentUnit.Health > 0)
            turnQueue.Enqueue(CurrentUnit);

        if (CurrentUnit.Team == EUnitTeam.player)
            PlayerTurn();
        else if (CurrentUnit.Team == EUnitTeam.enemy)
            EnemyTurn();
    }

    #endregion

    #region Buttons Handling
    /// <summary>
    /// Attack the selected enemy
    /// </summary>
    public void BTNAttack()
    {
        if (_selectedTarget >= 0 && _unitsInBattle[_selectedTarget].Team == EUnitTeam.enemy && _isPlayerActing)
        {
            _isPlayerActing = false;
            void HandleEndAttack()
            {
                CurrentUnit.OnEndAttack -= HandleEndAttack;
                NextTurn();
            }

            CurrentUnit.OnEndAttack += HandleEndAttack;
            CurrentUnit.StartAttackAnimation(_unitsInBattle[_selectedTarget]);
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
            NextTurn();
        }
    }

    public void BTNEscape()
    {
        if (_isPlayerActing)
        {
            _isPlayerActing = false;
            OnCloseBattle?.Invoke();
        }
    }

    public void BTNUseItem(ItemData item)
    {
        if (_isPlayerActing)
        {
            _isPlayerActing = false;
            OnUseItem?.Invoke(_unitsInBattle[_selectedTarget], item);
            NextTurn();
        }
    }
    #endregion

    #region Setup
    /// <summary>
    /// Setup all the mandatory needs for the battle to start
    /// </summary>
    private void SetupBattle()
    {
        _unitsInBattle.Clear();

        for (int i = 0; i < _playersPF.Length; i++)
        {
            var go = Instantiate(_playersPF[i], _spawnPointsPlayers[i].position, _spawnPointsPlayers[i].rotation);
            var unit = go.GetComponent<PlayerInCombat>();

            if (unit != null)
            {
                unit.OnPlayerDeath += HandlePlayerDeath;
                unit.SetSpeed(UnityEngine.Random.Range(0, 10));
                _unitsInBattle.Add(unit);
                OnCreateUnit?.Invoke(unit);
            }
        }

        for (int i = 0; i < _enemiesPF.Length; i++)
        {
            var go = Instantiate(_enemiesPF[i], _spawnPointsEnemies[i].position, _spawnPointsEnemies[i].rotation);
            var unit = go.GetComponent<EnemyInCombat>();

            if (unit != null)
            {
                unit.OnEnemyDeath += HandleEnemyDeath;
                unit.SetSpeed(UnityEngine.Random.Range(0, 10));
                _unitsInBattle.Add(unit);
                OnCreateUnit?.Invoke(unit);
            }
        }

        // Ordina per velocità e costruisci la queue
        var ordered = _unitsInBattle.OrderByDescending(u => u.Speed).ToList();
        turnQueue = new Queue<UnitBase>(ordered);

        // Primo turno
        NextTurn();

        _selectedTarget = _unitsInBattle.FindIndex(u => u.Team == EUnitTeam.enemy);
    }
    #endregion

    #region Victory and Death
    /// <summary>
    /// Handle the victory
    /// </summary>
    private void Victory()
    {
        if (!_unitsInBattle.Exists(p => p.Team == EUnitTeam.player))
        {
            Debug.Log("Enemies won!");
            _isFighting = false;
            OnCloseBattle?.Invoke();
        }
        else if (!_unitsInBattle.Exists(p => p.Team == EUnitTeam.enemy))
        {
            Debug.Log("Players won!");
            _isFighting = false;
            OnCloseBattle?.Invoke();
        }
    }
    /// <summary>
    /// Handle the player death
    /// </summary>
    private void HandlePlayerDeath()
    {
        if(_unitsInBattle.Find(u => u.Health <= 0) is PlayerInCombat player)
        {
            if (CurrentUnit == player)
            {
                Debug.Log("CurrentUnit Team " + CurrentUnit.Team);
                NextTurn();
            }
            _playersDeathPF.Add(player.gameObject);
            _unitsInBattle.Remove(player);
            Destroy(player.gameObject);
        }
    }
    /// <summary>
    /// Handle the enemy death
    /// </summary>
    private void HandleEnemyDeath()
    {
        if (_unitsInBattle.Find(u => u.Health <= 0) is EnemyInCombat enemy)
        {
            if (CurrentUnit == enemy)
            {
                Debug.Log("CurrentUnit Team " + CurrentUnit.Team);
                NextTurn();
            }

            _unitsInBattle.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }
    #endregion

    public IReadOnlyList<UnitBase> GetUnits() => _unitsInBattle;
}