using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIInventoryManager : MonoBehaviour
{
    [Header("Inventory settings")]
    [SerializeField] private GameObject m_inventory;

    [SerializeField] private GameObject m_itemPrefab;
    [SerializeField] private Transform m_itemUIParent;

    [Header("Dependency")]
    [SerializeField] private InventoryManager m_inventoryManager;

    private bool _isOpen;

    // --- Instance ---
    public static UIInventoryManager Instance { get; private set; }

    // --- Proprieties ---

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
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Inventory"].performed += OpenInventory;
        }
        if (m_inventoryManager != null)
        {
            m_inventoryManager.OnAddItem += CreateInvUI;
            m_inventoryManager.OnRemoveItem += UpdateItemUI;
        }

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        m_inventory.SetActive(false);
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Inventory"].performed -= OpenInventory;
        }

        if (m_inventoryManager != null)
        {
            m_inventoryManager.OnAddItem -= CreateInvUI;
            m_inventoryManager.OnRemoveItem -= UpdateItemUI;
        }
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        if (!_isOpen)
        {
            Time.timeScale = 0f;

            _isOpen = true;
            m_inventory.SetActive(_isOpen);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;

            _isOpen = false;
            m_inventory.SetActive(_isOpen);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void CreateInvUI()
    {
        foreach (Transform Child in m_itemUIParent)
            Destroy(Child.gameObject);
        foreach (ItemData itemData in m_inventoryManager.GetItems())
        {
            GameObject itemGO = Instantiate(m_itemPrefab, m_itemUIParent);

            itemGO.name = itemData.Item.name;

            itemGO.transform.Find("Item name").TryGetComponent(out TextMeshProUGUI itemNameTMP);
            Debug.Log($"Item name{itemData.Item.name}");
            itemNameTMP.text = itemData.Item.name;

            itemGO.transform.Find("Item qty").TryGetComponent(out TextMeshProUGUI itemQtyTMP);
            itemQtyTMP.text = $"x{itemData.Qty}";
        }
    }

    private void OpenInvCraftingUI()
    {
        CreateInvUI();
        foreach(GameObject Child in m_itemUIParent)
        {
            Button btn = Child.AddComponent<Button>();
            btn.onClick.AddListener(() => Debug.Log("Bottone premuto"));
        }
    }

    private void CloseInvCraftingUI()
    {

    }

    public void UpdateItemUI(ItemData item)
    {
        Transform Child = m_itemUIParent.Find(item.Item.name);
        if (item.Qty <= 0)
        {
            Destroy(Child.gameObject);
        }
        else
        {
            Child.transform.Find("Item qty").TryGetComponent(out TextMeshProUGUI itemQtyTMP);
            itemQtyTMP.text = $"x{item.Qty}";
        }
    }
}
