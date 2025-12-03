using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private Item m_item;
    [SerializeField] private int m_qty = 1;
    [SerializeField] private TextMeshProUGUI m_textInfoPickUp;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            m_textInfoPickUp.gameObject.SetActive(true);
            m_textInfoPickUp.text = $"Press [E] to pick up {m_item.name}";

            PlayerInputSingleton.Instance.Actions["Interact"].performed += PickUp;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            m_textInfoPickUp.gameObject.SetActive(false);
            PlayerInputSingleton.Instance.Actions["Interact"].performed -= PickUp;
        }
    }

    private void PickUp(InputAction.CallbackContext context)
    {
        PlayerInputSingleton.Instance.Actions["Interact"].performed -= PickUp;
        m_textInfoPickUp.gameObject.SetActive(false);
        InventoryManager.Instance.AddItemInInventory(m_item, m_qty);
        Destroy(gameObject);
    }
}