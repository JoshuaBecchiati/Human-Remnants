using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private UIPanel m_mainMenuPanel;

    [Header("Pause menu settings")]
    [SerializeField] private bool m_isPauseMenu;
    [SerializeField] private GameObject m_pauseCanvas;

    private Stack<UIPanel> panelStack = new Stack<UIPanel>();

    public static MenuManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (m_isPauseMenu)
            m_pauseCanvas.SetActive(false);
    }

    private void Start()
    {
        // inizializza lo stack con il MainMenu
        panelStack.Push(m_mainMenuPanel);
        m_mainMenuPanel.gameObject.SetActive(true);
        m_mainMenuPanel.OnEnter();

        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Back"].performed += BackInput;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Back"].performed -= BackInput;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        panelStack.Clear();
        // qui ti ri-agganci tutto
        FindReferences();
        panelStack.Push(m_mainMenuPanel);
    }

    private void FindReferences()
    {
        m_mainMenuPanel = GameObject.Find("MainMenu-Panel").GetComponent<UIPanel>();
        m_pauseCanvas = GameObject.Find("Pause Canvas");

        if (m_pauseCanvas != null)
        {
            m_pauseCanvas.SetActive(false);
            m_isPauseMenu = true;
        }
    }

    public void OpenPanel(UIPanel panel)
    {
        if (panelStack.Count > 0)
        {
            panelStack.Peek().OnExit();
            if(!panel.isSubMenu)
                panelStack.Peek().gameObject.SetActive(false);
        }

        panelStack.Push(panel);
        panel.gameObject.SetActive(true);
        panel.OnEnter();
    }

    public void Back()
    {
        if (panelStack.Count <= 1)
        {
            TogglePauseMenu();
            return;
        }

        UIPanel topPanel = panelStack.Peek();

        // Se è un subMenu, torniamo direttamente al MainMenu
        if (topPanel.isSubMenu)
        {
            while (panelStack.Count > 1)
            {
                UIPanel old = panelStack.Pop();
                old.OnExit();
                old.gameObject.SetActive(false);
            }

            UIPanel current = panelStack.Peek();
            current.gameObject.SetActive(true);
            current.OnEnter();
        }
        else
        {
            // comportamento normale: torna al panel precedente
            UIPanel old = panelStack.Pop();
            old.OnExit();
            old.gameObject.SetActive(false);

            UIPanel current = panelStack.Peek();
            current.gameObject.SetActive(true);
            current.OnEnter();
        }
    }

    public void BackInput(InputAction.CallbackContext ctx)
    {
        Back();
    }

    private void TogglePauseMenu()
    {
        if (m_isPauseMenu && GameEvents.IsInPause)
        {
            m_pauseCanvas.SetActive(false);
            GameEvents.SetGameState(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (m_isPauseMenu && !GameEvents.IsInPause)
        {
            m_pauseCanvas.SetActive(true);
            GameEvents.SetGameState(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void ChangeScene(int index)
    {
        LoadingScreenManager.Instance.LoadScene(index);
    }

    public void NewGame(Characters character)
    {
        LoadingScreenManager.Instance.LoadScene(1);
        SaveSystem.Instance.CreateNewSave(character.ToString());
    }
}
