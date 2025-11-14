using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

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
    [SerializeField] private GameObject m_exploreCamera;

    // --- Dependency ---

    // --- Private ---
    private GameObject _enemy;
    private BattleSettings _battleSettings;

    // --- Events ---
    public event Action<BattleSettings, IReadOnlyList<GameObject>> OnSetupBattle;


    public IReadOnlyList<GameObject> PlayersCombatPF =>
        m_players
            .Where(p => p.TryGetComponent<Player>(out _)) // filtra quelli che hanno lo script
            .Select(p => p.GetComponent<Player>().CombatPF) // prendi il prefab
            .ToList();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _battleScene.SetActive(false);
    }
    private void OnEnable()
    {
        GameEvents.OnBattleStart += BattleStart;
        GameEvents.OnBattleEnd += BattleClose;
    }
    private void OnDisable()
    {
        GameEvents.OnBattleStart -= BattleStart;
        GameEvents.OnBattleEnd -= BattleClose;
    }

    private void BattleStart(BattleSettings battleSettings, GameObject enemy)
    {
        // Set degli input da combattimento
        PlayerInputSingleton.Instance.CombatInput();

        _enemy = enemy;
        _battleSettings = battleSettings;

        // Disattiva il nemico nella scena esplorativa
        _enemy.GetComponent<Collider>().isTrigger = false;
        _enemy.SetActive(false);

        // Rende il cursore visibile
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Attiva la scena di combattimento e disattiva il player
        _battleScene.SetActive(true);
        m_currentPlayer.SetActive(false);
        m_exploreCamera.SetActive(false);

        OnSetupBattle?.Invoke(_battleSettings, PlayersCombatPF);
    }

    public void BattleClose()
    {
        _battleScene.SetActive(false);
        m_currentPlayer.SetActive(true);
        m_exploreCamera.SetActive(true);

        _enemy.SetActive(true);
        StartCoroutine(DisableCollider());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerInputSingleton.Instance.ExploreInput();
    }

    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(2f);
        _enemy.GetComponent<Collider>().isTrigger = true;
    }
}
