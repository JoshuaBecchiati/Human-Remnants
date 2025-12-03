using UnityEngine;
using UnityEngine.EventSystems;

public class SelectParentOnPointerDown : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("Se lasci null verrà cercato il parent AudioControlButton automaticamente")]
    [SerializeField] private GameObject m_parentToSelect;

    private void Reset()
    {
        // valore predefinito utile in editor: trova il parent con AudioControlButton
        if (m_parentToSelect == null)
        {
            var acb = GetComponentInParent<AudioControlButton>();
            if (acb != null) m_parentToSelect = acb.gameObject;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_parentToSelect == null)
        {
            var acb = GetComponentInParent<AudioControlButton>();
            if (acb != null) m_parentToSelect = acb.gameObject;
        }

        if (m_parentToSelect == null) return;

        // Forza la selezione sul parent IMMEDIATAMENTE
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(m_parentToSelect);
    }
}