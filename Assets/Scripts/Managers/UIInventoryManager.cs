using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject m_inventory;
    [SerializeField] private Transform m_inventoryTransform;

    [SerializeField] private GameObject m_itemSlotPrefab;

    private bool _isInventoryOpen; // True = open, False = closed

    private void Start()
    {
        m_inventory.SetActive(false);

        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Inventory"].performed += ToggleInventory;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnAddItem += CreateItemSlots;
            InventoryManager.Instance.OnRemoveItem += CreateItemSlots;
        }

        CreateItemSlots();
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Inventory"].performed -= ToggleInventory;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnAddItem -= CreateItemSlots;
            InventoryManager.Instance.OnRemoveItem -= CreateItemSlots;
        }
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        if (_isInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    private void OpenInventory()
    {
        _isInventoryOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;

        m_inventory.SetActive(true);
    }

    private void CloseInventory()
    {
        _isInventoryOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        m_inventory.SetActive(false);
    }

    private void CreateItemSlots()
    {
        foreach (Transform child in m_inventoryTransform)
            Destroy(child.gameObject);

        foreach (ItemData item in InventoryManager.Instance.GetItems())
        {
            GameObject itemSlot = Instantiate(m_itemSlotPrefab, m_inventoryTransform);

            // Object name
            itemSlot.transform.Find("Item name").GetComponent<TextMeshProUGUI>().text = item.Item.name;

            // Object quantity
            itemSlot.transform.Find("Item qty").GetComponent<TextMeshProUGUI>().text = "x" + item.Qty;
        }
    }
}
