using UnityEngine;

public class ChangePanelBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    [SerializeField] private UIPanel m_panel;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        MenuManager.Instance.OpenPanel(m_panel);
    }
}
