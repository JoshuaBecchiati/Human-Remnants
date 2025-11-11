using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager2 : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private CinemachineBrain cinemachineBrain;
    private bool isPaused = false;

    private void Start()
    {
        Time.timeScale = 1f;
        Debug.Log("Start");
        pauseMenu.SetActive(false);
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Escape"].performed += TogglePause;
        }
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Escape"].performed -= TogglePause;
        }
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        Debug.Log("Pausa");
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(isPaused);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cinemachineBrain.enabled = !isPaused;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(isPaused);

        Cursor.visible = false;
        cinemachineBrain.enabled = !isPaused;
    }

    public void MenuButton()
    {
        SceneManager.LoadScene(0);
    }
}
