using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    // --- Inspector References ---
    [Header("Scenes")]
    [SerializeField] private GameObject _battleScene;

    [Header("Players")]
    [SerializeField] private List<GameObject> m_players;
    [SerializeField] private GameObject m_currentPlayer;

    public IReadOnlyList<GameObject> PlayersCombatPF =>
        m_players
            .Where(p => p.TryGetComponent<Player>(out _)) // filtra quelli che hanno lo script
            .Select(p => p.GetComponent<Player>().CombatPF) // prendi il prefab
            .ToList();

    private GameObject _currentBattle;


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
        GameEvents.OnBattleStart += BattleStart;
    }
    private void OnDisable()
    {
        GameEvents.OnBattleStart -= BattleStart;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance)
            BattleManager.Instance.OnCloseBattle -= CloseBattle;

        if (Instance == this) Instance = null;
    }

    private void BattleStart(BattleSettings battleSettings)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _battleScene.SetActive(true);
        m_currentPlayer.SetActive(false);

        NewBattleManager.Instance.SetupBattle(battleSettings, PlayersCombatPF);

    }

    public void EnterBattle(GameObject battleScene, GameObject player)
    {
        _currentBattle = battleScene;
        m_currentPlayer = player;

        _currentBattle.SetActive(true);

        m_currentPlayer.SetActive(false);

        BattleManager.Instance.OnCloseBattle += CloseBattle;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseBattle()
    {
        if (_currentBattle != null)
            _currentBattle.SetActive(false);
        if (m_currentPlayer != null)
            m_currentPlayer.SetActive(true);

        m_currentPlayer = null;
        _currentBattle = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
