using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [SerializeField] private int _sceneToLoad;
    [SerializeField] private Animator _animator;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(int sceneIndex)
    {
        _sceneToLoad = sceneIndex;
        _animator.SetTrigger("Load");
    }

    public void StartLoading()
    {
        SceneManager.LoadScene(_sceneToLoad);
    }
}
