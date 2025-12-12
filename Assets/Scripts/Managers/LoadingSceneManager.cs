using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [SerializeField] private int m_sceneToLoad;
    [SerializeField] private Animator m_animator;

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

    private void OnValidate()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    public void LoadScene(int sceneIndex)
    {
        m_sceneToLoad = sceneIndex;
        m_animator.SetTrigger("Load");
    }

    public void StartLoading()
    {
        SceneManager.LoadScene(m_sceneToLoad);
    }
}
