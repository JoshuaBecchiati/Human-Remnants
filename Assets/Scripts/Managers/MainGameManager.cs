using System.Collections;
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

    // --- Private ---
    private GameObject _enemy;

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
    }
    private void OnEnable()
    {
        GameEvents.OnBattleStart += BattleStart;
        GameEvents.OnBattleEnd += BattelClose;
    }
    private void OnDisable()
    {
        GameEvents.OnBattleStart -= BattleStart;
        GameEvents.OnBattleEnd -= BattelClose;
    }

    private void BattleStart(BattleSettings battleSettings, GameObject enemy)
    {
        _enemy = enemy;

        _enemy.GetComponent<Collider>().isTrigger = false;
        _enemy.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _battleScene.SetActive(true);
        m_currentPlayer.SetActive(false);

        NewBattleManager.Instance.SetupBattle(battleSettings, PlayersCombatPF);
    }

    public void BattelClose()
    {
        _battleScene.SetActive(false);
        m_currentPlayer.SetActive(true);

        _enemy.SetActive(true);
        StartCoroutine(DisableCollider());

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(2f);
        _enemy.GetComponent<Collider>().isTrigger = true;
    }
}
