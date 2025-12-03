using UnityEngine;

public class QuitBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        MenuManager.Instance.QuitApplication();
    }
}
