using UnityEngine;

public class ChangeSceneBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    [SerializeField] private int m_indexScene;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        Time.timeScale = 1f;
        MenuManager.Instance.ChangeScene(m_indexScene);
    }
}
