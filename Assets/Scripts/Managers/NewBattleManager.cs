using System.Collections;
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

    public static NewBattleManager Instance { get; private set; }

    private int _indexTarget = 0;
    private int _indexCurrentUnit = 0;

    private bool _isPlayerActing = false;

    private UnitBase _currentUnit;
    private Camera _battleCamera;

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
            e.SetSpeed(Random.Range(10, 20));
            _unitsInBattle.Add(e);
        }

        // Ording the list by the speed
        _unitsInBattle = _unitsInBattle.OrderBy(x => x.Speed).ToList();

        // Start the first turn
        _currentUnit = _unitsInBattle[0];
        _currentUnit.StartTurn();
    }

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

        do
        {
            _indexCurrentUnit += 1;
        } while (_unitsInBattle[_indexCurrentUnit].Health <= 0);


        _currentUnit = _unitsInBattle[_indexCurrentUnit];

        _currentUnit.StartTurn();
    }
}
