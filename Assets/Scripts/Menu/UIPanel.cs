using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    [Header("Sub-panel settings")]
    [SerializeField] private UIPanel m_defaultSubPanel;
    [SerializeField] private bool m_isSubMenu;

    public bool isSubMenu => m_isSubMenu;

    public virtual void OnEnter()
    {
        if (m_defaultSubPanel != null)
        {
            MenuManager.Instance.OpenPanel(m_defaultSubPanel);
        }
    }

    public virtual void OnExit() { }
}