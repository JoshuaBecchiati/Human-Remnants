using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private UIPanel mainMenuPanel;

    [Header("Pause menu settings")]
    [SerializeField] private bool m_isPauseMenu;
    [SerializeField] private GameObject m_pauseCanvas;

    private Stack<UIPanel> panelStack = new Stack<UIPanel>();

    public static MenuManager Instance;

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
        panelStack.Push(mainMenuPanel);
        mainMenuPanel.gameObject.SetActive(true);
        mainMenuPanel.OnEnter();

        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Back"].performed += Back;
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Back"].performed -= Back;
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

    public void Back(InputAction.CallbackContext ctx)
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
}
