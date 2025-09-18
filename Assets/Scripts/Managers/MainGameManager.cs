using UnityEngine;

[DefaultExecutionOrder(-10)]
public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    private GameObject _currentPlayer;
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
        GameEvents.OnBattleEnter += EnterBattle;
    }
    private void OnDisable()
    {
        GameEvents.OnBattleEnter -= EnterBattle;
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance)
            BattleManager.Instance.OnCloseBattle -= CloseBattle;

        if (Instance == this) Instance = null;
    }

    public void EnterBattle(GameObject battleScene, GameObject player)
    {
        _currentBattle = battleScene;
        _currentPlayer = player;

        _currentBattle.SetActive(true);

        _currentPlayer.SetActive(false);

        BattleManager.Instance.OnCloseBattle += CloseBattle;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseBattle()
    {
        if (_currentBattle != null)
            _currentBattle.SetActive(false);
        if (_currentPlayer != null)
            _currentPlayer.SetActive(true);

        _currentPlayer = null;
        _currentBattle = null;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
