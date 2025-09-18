using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private GameObject _deathUI;
    [SerializeField] private GameObject _statsUI;
    [SerializeField] private Player _player;


    private void Awake()
    {
        _player.OnDeath += HandlePlayerDeath;

        _deathUI.SetActive(false);
    }

    private void OnDestroy()
    {
        _player.OnDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _deathUI.SetActive(true);
        _statsUI.SetActive(false);
    }

    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
