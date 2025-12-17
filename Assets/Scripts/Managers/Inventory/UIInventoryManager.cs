using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventoryManager : MonoBehaviour
{
    // --- Inspector ---
    [SerializeField] private GameObject m_inventory;
    [SerializeField] private Transform m_inventoryTransform;
    [SerializeField] private GameObject m_itemSlotPrefab;
    [SerializeField] private Transform m_itemDesc;

    // --- Private ---
    private bool _isInventoryOpen; // True = open, False = closed

    // --- Proprierties ---
    public Transform InventoryTransform => m_inventoryTransform;

    // --- Instance ---
    public static UIInventoryManager Instance { get; private set; }

    private void Awake()
    {
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
        if (GameEvents.CanOpenInventory) return;

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

        GameEvents.SetInventoryState(true);
        m_inventory.SetActive(true);
    }

    private void CloseInventory()
    {
        _isInventoryOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;

        HideDescription();

        GameEvents.SetInventoryState(false);
        m_inventory.SetActive(false);
    }

    private void CreateItemSlots()
    {
        foreach (Transform child in m_inventoryTransform)
            Destroy(child.gameObject);

        foreach (ItemData item in InventoryManager.Instance.GetItems())
        {
            if (item == null) continue;
            CreateItemSlot(item, m_inventoryTransform);
        }
    }

    public void CreateItemSlot(ItemData itemData, Transform parent)
    {
        GameObject itemSlot = Instantiate(m_itemSlotPrefab, parent);

        // Object name
        itemSlot.transform.Find("Item name").GetComponent<TextMeshProUGUI>().text = itemData.Item.name;
        itemSlot.transform.Find("Item name highlighted").GetComponent<TextMeshProUGUI>().text = itemData.Item.name;

        // Object quantity
        itemSlot.transform.Find("Item qty").GetComponent<TextMeshProUGUI>().text = "x" + itemData.Qty;
        itemSlot.transform.Find("Item qty highlighted").GetComponent<TextMeshProUGUI>().text = "x" + itemData.Qty;

        // Object Sprite
        itemSlot.transform.Find("Sprite").GetComponent<Image>().sprite = itemData.Item.icon;
    }

    public void CreateDropItemSlot(ItemData itemData, Transform parent, GameObject prefab)
    {
        GameObject itemSlot = Instantiate(prefab, parent);

        // Object name
        itemSlot.transform.Find("Item name").GetComponent<TextMeshProUGUI>().text = itemData.Item.name;

        // Object quantity
        itemSlot.transform.Find("Item qty").GetComponent<TextMeshProUGUI>().text = "x" + itemData.Qty;

        // Object Sprite
        itemSlot.transform.Find("Sprite").GetComponent<Image>().sprite = itemData.Item.icon;
    }

    public void ShowDescription(Item item)
    {
        m_itemDesc.Find("Desc").GetComponent<TextMeshProUGUI>().text = item.description;

        m_itemDesc.Find("Desc_Name").GetComponent<TextMeshProUGUI>().text = item.name;

        Image icon = m_itemDesc.Find("Desc_Icon").GetComponent<Image>();
        Color alpha = icon.color;
        icon.sprite = item.icon;
        alpha.a = 1f;
        icon.color = alpha;
    }

    public void HideDescription()
    {
        m_itemDesc.Find("Desc").GetComponent<TextMeshProUGUI>().text = string.Empty;

        m_itemDesc.Find("Desc_Name").GetComponent<TextMeshProUGUI>().text = string.Empty;

        Image icon = m_itemDesc.Find("Desc_Icon").GetComponent<Image>();
        Color alpha = icon.color;
        icon.sprite = null;
        alpha.a = 0f;
        icon.color = alpha;
    }
}