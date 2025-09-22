using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject m_inventory;

    [SerializeField] private GameObject m_itemPrefab;
    [SerializeField] private Transform m_itemUIParent;

    [Header("Dependency")]
    [SerializeField] private InventoryManager m_inventoryManager;

    private bool _isOpen;

    // --- Instance ---
    public UIInventoryManager Instance { get; private set; }

    // --- Proprieties ---

    private void Start()
    {
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Inventory"].performed += OpenInventory;
        }
        if (m_inventoryManager != null)
        {
            m_inventoryManager.OnAddItem += CreateInvUI;
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
            Debug.Log("Inventory opened");
        }
        else
        {
            Time.timeScale = 1f;
            _isOpen = false;
            m_inventory.SetActive(_isOpen);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Inventory closed");
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

            itemGO.gameObject.transform.Find("Item name").TryGetComponent(out TextMeshProUGUI itemNameTMP);
            Debug.Log($"Item name{itemData.Item.name}");
            itemNameTMP.text = itemData.Item.name;

            itemGO.gameObject.transform.Find("Item qty").TryGetComponent(out TextMeshProUGUI itemQtyTMP);
            itemQtyTMP.text = $"x{itemData.Qty}";
        }
    }
}
