using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu principali")]
    [SerializeField] private List<GameObject> m_menuScenes; // Es. MainMenu, Options, ecc.
    [SerializeField] private List<GameObject> m_firstButtons; // Primo bottone per ogni menu
    [SerializeField] private EventSystem m_eventSystem;

    [Header("Sottomenu di Options")]
    [SerializeField] private List<GameObject> m_submenus; // General, Gameplay, Sounds, Controls, ecc.
    [SerializeField] private List<GameObject> m_firstSubmenuButtons; // Primo bottone per ogni sottomenu (Master, ecc.)

    private Stack<int> m_menuHistory = new Stack<int>();
    private bool m_isInSubmenu = false;
    private int m_currentMenuIndex = 0;
    private int m_currentSubmenuIndex = -1; // quale submenu è aperto (-1 = nessuno)

    private void Awake()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Back"].performed += BackMenuScene;
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Back"].performed -= BackMenuScene;
    }

    private void Start()
    {
        foreach (GameObject go in m_menuScenes)
            go.SetActive(false);

        m_menuScenes[0].SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_firstButtons[0]);
        m_currentMenuIndex = 0;
    }

    private void BackMenuScene(InputAction.CallbackContext context)
    {
        if (m_isInSubmenu)
        {
            ExitSubmenu();
        }
        else if (m_menuHistory.Count > 0)
        {
            int previousIndex = m_menuHistory.Pop();
            ChangeMenuScene(previousIndex, false);
        }
    }

    public void BTNChangeMenuScene(int sceneMenuIndex)
    {
        ChangeMenuScene(sceneMenuIndex);
    }

    private void ChangeMenuScene(int sceneMenuIndex, bool addToHistory = true)
    {
        if (sceneMenuIndex == m_currentMenuIndex)
            return;

        if (addToHistory)
            m_menuHistory.Push(m_currentMenuIndex);

        foreach (GameObject go in m_menuScenes)
            go.SetActive(false);

        m_menuScenes[sceneMenuIndex].SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_firstButtons[sceneMenuIndex]);
        m_currentMenuIndex = sceneMenuIndex;

        m_isInSubmenu = false;
        m_currentSubmenuIndex = -1;
    }

    // --- SUBMENU GENERICI ---

    public void EnterSubmenu(int submenuIndex)
    {
        if (submenuIndex < 0 || submenuIndex >= m_submenus.Count)
            return;

        // Chiudi eventuale submenu già aperto
        foreach (GameObject go in m_submenus)
            go.SetActive(false);

        m_submenus[submenuIndex].SetActive(true);
        m_isInSubmenu = true;
        m_currentSubmenuIndex = submenuIndex;

        // Seleziona primo bottone del submenu
        if (submenuIndex < m_firstSubmenuButtons.Count && m_firstSubmenuButtons[submenuIndex] != null)
            m_eventSystem.SetSelectedGameObject(m_firstSubmenuButtons[submenuIndex]);
    }

    public void ExitSubmenu()
    {
        if (!m_isInSubmenu)
            return;

        m_isInSubmenu = false;
        m_currentSubmenuIndex = -1;

        // Torna a selezionare il pulsante padre in Options (es. Sounds)
        m_eventSystem.SetSelectedGameObject(m_firstButtons[m_currentMenuIndex]);
    }

    // --- SCENE MANAGEMENT ---

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
