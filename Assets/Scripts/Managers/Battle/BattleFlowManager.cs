using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private List<UnitBase> _units;

    // --- Events ---
    public event Action<IReadOnlyList<GameObject>, IReadOnlyList<GameObject>> OnSetupBattle;
    public event Action<UnitBase> OnCreateUnit;

    // --- Public ---
    public List<GameObject> PlayersCombatPF =>
        m_players
            .Where(p => p.TryGetComponent<Player>(out _)) // Filter only with Player script
            .Select(p => p.GetComponent<Player>().CombatPF) // Take prefab
            .ToList();

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
    #endregion

    #region Handle start battle
    private void BattleStart(BattleSettings battleSettings, GameObject enemy)
    {
        _enemy = enemy;
        _battleSettings = battleSettings;

        StartCoroutine(FadeInBattle());
    }

    /// <summary>
    /// Transition to enter the battle
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeInBattle()
    {
        m_blackScreen.gameObject.SetActive(true);

        // Initialize camera values
        float targetFOV = 140f;
        float startFOV = m_explorationCamera.m_Lens.FieldOfView;
        float t = 0f;
        float duration = 0.45f;

        // Changing camera FOV
        while (t < duration)
        {
            t += Time.deltaTime;
            m_explorationCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t / duration);
            yield return null;
        }

        // Fade in black screen transition
        t = 0f;
        duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            m_blackScreen.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // Resete exploration camera
        m_explorationCamera.m_Lens.FieldOfView = startFOV;

        InitializeStartBattle();
    }

    private void InitializeStartBattle()
    {
        // Set combat inputs
        PlayerInputSingleton.Instance.CombatInput();

        // Disable enemy in scene
        _enemy.GetComponent<Collider>().isTrigger = false;
        _enemy.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Activate combat scene, disable exploration player
        m_BattleUI.SetActive(true);
        m_battleScene.SetActive(true);
        m_currentPlayer.SetActive(false);
        m_exploreCamera.SetActive(false);

        // Prefab instantiate and event to start the battle
        IReadOnlyList<GameObject> enemies = InstantiatePrefabs(_battleSettings.enemies.ToList());
        IReadOnlyList<GameObject> players = InstantiatePrefabs(PlayersCombatPF);

        OnSetupBattle?.Invoke(players, enemies);
    }

    /// <summary>
    /// Instatiate prefab and healthbar of every unit in the list
    /// </summary>
    /// <param name="prefabs"></param>
    /// <returns></returns>
    private IReadOnlyList<GameObject> InstantiatePrefabs(List<GameObject> prefabs)
    {
        List<GameObject> prefabList = new();

        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject go = Instantiate(prefabs[i]);
            UnitBase u = go.GetComponentInChildren<UnitBase>();
            Player p = m_players[i].GetComponent<Player>();

            // Sync player exploration health with combat health
            if (u.Team == UnitTeam.Player && u.Health >= p.Health)
            {
                u.SetHealth(p.Health);
            }

            OnCreateUnit.Invoke(u);
            prefabList.Add(go);
        }

        return prefabList;
    }
    #endregion

    #region Handle close battle
    public void BattleClose(BattleResult winner, List<UnitBase> units)
    {
        _units = units;

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

        SetEndBattleHealth(_units);
    }

    /// <summary>
    /// Sync player combat health and exploration health after battle
    /// </summary>
    /// <param name="units"></param>
    private void SetEndBattleHealth(List<UnitBase> units)
    {
        foreach (UnitBase u in _units)
        {
            if (u.Team == UnitTeam.Enemy) continue;

            Player player = m_players
                .Select(go => go.GetComponent<Player>())
                .FirstOrDefault(p => p != null && p.Name == u.Name);

            if (player != null)
            {
                player.SetHealth(u.Health);
            }
        }
    }

    /// <summary>
    /// Active victory player screen
    /// </summary>
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

    /// <summary>
    /// Active game over screen
    /// </summary>
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

        // Fade in black screen to transition out of combat
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

        // Destroy every unit prefab
        foreach (UnitBase u in _units)
            Destroy(u.transform.parent.gameObject);

        // Fade out black screen to transition in exploration
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

        GameEvents.SetBattleState(false);
    }

    /// <summary>
    /// Disable enemy collider for a set time
    /// to avoid immedate play battle in case of escape from it
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(2f);
        _enemy.GetComponent<Collider>().isTrigger = true;
    }
    #endregion
}
