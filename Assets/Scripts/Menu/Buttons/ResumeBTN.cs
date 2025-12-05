using UnityEngine;

public class ResumeBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        Debug.Log("Back");
        MenuManager.Instance.Back();
    }
}
