using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : SaveableObject
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
        SaveSystem.Instance.CurrentSave.collectedItems.Add(uniqueID);
        m_textInfoPickUp.gameObject.SetActive(false);
        InventoryManager.Instance.AddItemInInventory(m_item, m_qty);
        gameObject.SetActive(false);
    }

    public override void LoadState(SaveData save)
    {
        if (save.collectedItems.Contains(uniqueID))
            gameObject.SetActive(false);
        else if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public override void SaveState(SaveData save)
    {
        if (!save.collectedItems.Contains(uniqueID) && !gameObject.activeSelf)
            save.collectedItems.Add(uniqueID);
    }
}