using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIInventoryManager : MonoBehaviour
{
    // --- Inspector ---
    [SerializeField] private GameObject m_inventory;
    [SerializeField] private Transform m_inventoryTransform;
    [SerializeField] private GameObject m_itemSlotPrefab;

    // --- Private ---
    private bool _isInventoryOpen; // True = open, False = closed

    // --- Proprierties ---
    public Transform InventoryTransform => m_inventoryTransform;

    // --- Instance ---
    public static UIInventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        m_inventory.SetActive(false);

        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Inventory"].performed += OnToggleInventory;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnAddItem += CreateItemSlots;
            InventoryManager.Instance.OnRemoveItem += CreateItemSlots;
        }

        GameEvents.OnOpenCrafting += OpenInventory;
        GameEvents.OnCloseCrafting += CloseInventory;

        CreateItemSlots();
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
            PlayerInputSingleton.Instance.Actions["Inventory"].performed -= OnToggleInventory;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnAddItem -= CreateItemSlots;
            InventoryManager.Instance.OnRemoveItem -= CreateItemSlots;
        }

        GameEvents.OnOpenCrafting -= OpenInventory;
        GameEvents.OnCloseCrafting -= CloseInventory;
    }

    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        if (GameEvents.IsInCrafting) return;

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
            CreateItemSlot(item, m_inventoryTransform);
        }
    }

    public void CreateItemSlot(ItemData itemData, Transform parent)
    {
        GameObject itemSlot = Instantiate(m_itemSlotPrefab, parent);

        // Object name
        itemSlot.transform.Find("Item name").GetComponent<TextMeshProUGUI>().text = itemData.Item.name;

        // Object quantity
        itemSlot.transform.Find("Item qty").GetComponent<TextMeshProUGUI>().text = "x" + itemData.Qty;

        // Object Sprite
        itemSlot.transform.Find("Sprite").GetComponent<Image>().sprite = itemData.Item.icon;
    }
}
