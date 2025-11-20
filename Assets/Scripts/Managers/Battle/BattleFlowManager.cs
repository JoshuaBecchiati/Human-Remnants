using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class BattleFlowManager : MonoBehaviour
{
    // --- Instance ---
    public static BattleFlowManager Instance { get; private set; }

    // --- Inspector References ---
    [Header("Scenes")]
    [SerializeField] private GameObject m_battleScene;

    [Header("UI")]
    [SerializeField] private GameObject m_BattleUI;
    [SerializeField] private GameObject m_VictoryUI;
    [SerializeField] private GameObject m_LoseUI;

    [Header("Players")]
    [SerializeField] private List<GameObject> m_players;
    [SerializeField] private GameObject m_currentPlayer;
    [SerializeField] private GameObject m_exploreCamera;

    [Header("Victory screen")]
    [SerializeField] private GameObject m_itemSlot;
    [SerializeField] private Transform m_transformItemSlot;

    [Header("Camera effects")]
    [SerializeField] private CanvasGroup m_blackScreen;
    [SerializeField] private CinemachineVirtualCamera m_explorationCamera;

    // --- Private ---
    private GameObject _enemy;
    private BattleSettings _battleSettings;

    // --- Events ---
    public event Action<IReadOnlyList<GameObject>, IReadOnlyList<GameObject>> OnSetupBattle;

    // --- Public ---
    public List<GameObject> PlayersCombatPF =>
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
    }
    private void OnEnable()
    {
        GameEvents.OnBattleStart += BattleStart;
        GameEvents.OnBattleEnd += BattleClose;

        m_battleScene.SetActive(false);
        m_VictoryUI.SetActive(false);
        m_BattleUI.SetActive(false);
    }
    private void OnDisable()
    {
        GameEvents.OnBattleStart -= BattleStart;
        GameEvents.OnBattleEnd -= BattleClose;
    }

    private void BattleStart(BattleSettings battleSettings, GameObject enemy)
    {
        _enemy = enemy;
        _battleSettings = battleSettings;

        StartCoroutine(FadeInBattle());
    }

    private IEnumerator FadeInBattle()
    {
        m_blackScreen.gameObject.SetActive(true);

        float targetFOV = 140f;
        float startFOV = m_explorationCamera.m_Lens.FieldOfView;
        float t = 0f;
        float duration = 0.45f;

        while (t < duration)
        {
            t += Time.deltaTime;
            m_explorationCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t / duration);
            yield return null;
        }

        m_explorationCamera.m_Lens.FieldOfView = targetFOV;

        t = 0f;
        duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            m_blackScreen.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        m_explorationCamera.m_Lens.FieldOfView = targetFOV;

        yield return new WaitForSeconds(0.5f);

        m_explorationCamera.m_Lens.FieldOfView = startFOV;
        InitializeStartBattle();
    }

    private void InitializeStartBattle()
    {
        // Set degli input da combattimento
        PlayerInputSingleton.Instance.CombatInput();

        // Disattiva il nemico nella scena esplorativa
        _enemy.GetComponent<Collider>().isTrigger = false;
        _enemy.SetActive(false);

        // Rende il cursore visibile
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Attiva la scena di combattimento e disattiva il player
        m_BattleUI.SetActive(true);
        m_battleScene.SetActive(true);
        m_currentPlayer.SetActive(false);
        m_exploreCamera.SetActive(false);

        // Istanziamento e evento per inizio della battaglia
        IReadOnlyList<GameObject> enemies = InstantiatePrefabs(_battleSettings.enemies.ToList());
        IReadOnlyList<GameObject> players = InstantiatePrefabs(PlayersCombatPF);

        OnSetupBattle?.Invoke(players, enemies);
    }

    private IReadOnlyList<GameObject> InstantiatePrefabs(List<GameObject> prefabs)
    {
        List<GameObject> prefabList = new();

        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject go = Instantiate(prefabs[i]);
            prefabList.Add(go);
        }

        return prefabList;
    }

    public void BattleClose(BattleResult winner)
    {
        m_BattleUI.SetActive(false);
        switch (winner)
        {
            case BattleResult.Player:
                PlayerWin();
                break;
            case BattleResult.Enemy:
                PlayerLose();
                break;
            case BattleResult.Escape:
                BattleCloseBTN();
                break;
        }
    }

    private void PlayerWin()
    {
        m_VictoryUI.SetActive(true);

        foreach (Transform child in m_transformItemSlot)
            Destroy(child.gameObject);

        foreach (ItemData itemData in _battleSettings.drops)
        {
            InventoryManager.Instance.AddItemInInventory(itemData.Item, itemData.Qty);
            UIInventoryManager.Instance.CreateItemSlot(itemData, m_transformItemSlot);
        }
    }

    private void PlayerLose()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        m_LoseUI.SetActive(true);
    }

    public void BattleCloseBTN()
    {
        StartCoroutine(FadeOutBattle());


    }

    private IEnumerator FadeOutBattle()
    {
        m_VictoryUI.SetActive(false);

        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;
            m_blackScreen.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        m_battleScene.SetActive(false);
        m_BattleUI.SetActive(true);
        m_currentPlayer.SetActive(true);
        m_exploreCamera.SetActive(true);

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            m_blackScreen.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        _enemy.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerInputSingleton.Instance.ExploreInput();

        yield return new WaitForSeconds(2f);

        StartCoroutine(DisableCollider());
    }

    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(2f);
        _enemy.GetComponent<Collider>().isTrigger = true;
    }
}
