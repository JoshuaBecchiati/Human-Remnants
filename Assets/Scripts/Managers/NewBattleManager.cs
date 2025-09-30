using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBattleManager : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPointsPlayers;
    [SerializeField] private Transform[] _spawnPointsEnemies;

    [Header("Battle List")]
    [SerializeField] private List<GameObject> _unitsInBattle;

    public static NewBattleManager Instance { get; private set; }

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

    public void SetupBattle(BattleSettings battleSettings, IReadOnlyList<GameObject> playersPf)
    {
        _battleCamera = Camera.main;

        for (int i = 0; i < playersPf.Count; i++)
        {
            GameObject go = Instantiate(playersPf[i], _spawnPointsPlayers[i]);
            _unitsInBattle.Add(go);
        }

        for (int i = 0; i < battleSettings.enemies.Length; i++)
        {
            GameObject go = Instantiate(battleSettings.enemies[i], _spawnPointsEnemies[i]);
            _unitsInBattle.Add(go);
        }
    }

    private void SelectTarget(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        int direction;

        if (dir.x > 0.5f) direction = 1;        // Destra
        else if (dir.x < -0.5f) direction = -1; // Sinistra
        else return;
    }
}
