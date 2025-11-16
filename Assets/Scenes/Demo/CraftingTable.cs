using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class CraftingTable : MonoBehaviour
{
    [SerializeField] private CraftingRecipe m_recipe;
    [SerializeField] private GameObject m_craftMenu;
    [SerializeField] private GameObject m_infoText;
    [SerializeField] private GameObject m_backButton;
    [SerializeField] private TextMeshProUGUI m_infoLabel;
    [SerializeField] private GameObject[] m_craftingSlots;

    // --- Private ---
    private bool _playerNearby;
    private bool _isCraftingOpen; // True: opened, False: Closed
    private bool _isChoosing;
    private GameObject _activeSlot;
    private ItemData[] _items = new ItemData[2];
    private int _slotIndex;

    #region Unity methods
    private void Awake()
    {
        m_craftMenu.SetActive(false);
        m_infoText.SetActive(false);

        InputSystem.onDeviceChange += OnDeviceChange;
        UpdateInfoText(); // iniziale
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = true;
        m_infoText.SetActive(true);

        UpdateInfoText();

        PlayerInputSingleton.Instance.Actions["Interact"].performed += OnToggleCrafting;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;

        m_infoText.SetActive(false);

        SetCraftingOpen(false);

        for (int i = 0; i < m_craftingSlots.Length; i++)
            m_craftingSlots[i].GetComponent<Image>().sprite = null;

        PlayerInputSingleton.Instance.Actions["Interact"].performed -= OnToggleCrafting;
    }
    #endregion

    #region Open and Close handlers
    /// <summary>
    /// Open or close the crafting station if the player is near
    /// </summary>
    /// <param name="context"></param>
    private void OnToggleCrafting(InputAction.CallbackContext context)
    {
        if (!_playerNearby) return; // sicurezza in più

        SetCraftingOpen(!_isCraftingOpen);
    }

    /// <summary>
    /// Close the crafting station
    /// </summary>
    private void SetCraftingOpen(bool open)
    {
        _isCraftingOpen = open;
        GameEvents.SetCraftingState(open);

        if (_playerNearby)
            m_infoText.SetActive(!open);

        m_craftMenu.SetActive(open);

        Cursor.visible = open;
        Cursor.lockState = open ?
            CursorLockMode.None :
            CursorLockMode.Locked;
    }
    #endregion

    #region Crafting
    public void Craft()
    {
        if(CraftManager.Instance.Craft(_items))
        {
            for (int i = 0; i < m_craftingSlots.Length; i++)
            {
                m_craftingSlots[i].GetComponent<Image>().sprite = null;
                _items[i] = null;
            }
        }
    }
    #endregion

    #region Text handlers
    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
            UpdateInfoText();
    }

    private void UpdateInfoText()
    {
        string keyPrompt = "E"; // default tastiera

        if (Gamepad.current != null)
        {
            if (Gamepad.current is XInputController || Gamepad.current.displayName.Contains("Xbox", StringComparison.OrdinalIgnoreCase))
                keyPrompt = "X";
            else if (Gamepad.current.displayName.Contains("DualShock", StringComparison.OrdinalIgnoreCase))
                keyPrompt = "Square";
            else
                keyPrompt = "Button";
        }

        if (m_infoLabel != null)
            m_infoLabel.text = $"Premi [{keyPrompt}] per interagire";
    }
    #endregion

    #region Buttons
    public void ChooseItemBTN()
    {
        if (_isChoosing) return;

        _isChoosing = true;

        m_backButton.gameObject.SetActive(_isChoosing);
        GameEvents.SetCraftingOpened(_isChoosing);
        ToggleCraftingButtons(_isChoosing);
    }

    public void BackMenuBTN()
    {
        if (!_isChoosing) return;

        _isChoosing = false;

        m_backButton.gameObject.SetActive(_isChoosing);
        GameEvents.SetCraftingOpened(_isChoosing);
        ToggleCraftingButtons(_isChoosing);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ToggleCraftingButtons(bool enable)
    {
        foreach (Transform child in UIInventoryManager.Instance.InventoryTransform)
        {
            Transform currentChild = child;

            if (enable)
            {
                if (!currentChild.TryGetComponent<Button>(out var btn))
                    btn = currentChild.gameObject.AddComponent<Button>();

                btn.targetGraphic = currentChild.GetComponent<Image>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => GetItem(currentChild));
            }
            else
            {
                if (currentChild.TryGetComponent<Button>(out var btn))
                    Destroy(btn);
            }
        }
    }

    private void GetItem(Transform item)
    {
        string itemName = item.Find("Item name").GetComponent<TextMeshProUGUI>().text;

        ItemData itemData = InventoryManager.FindItemByName(itemName);

        _items[_slotIndex] = itemData;

        PutItemInActiveSlot(itemData);

        BackMenuBTN();
    }

    public void SelectSlot(int index)
    {
        _slotIndex = index;
        _activeSlot = m_craftingSlots[_slotIndex];
    }

    public void PutItemInActiveSlot(ItemData item)
    {
        if (_activeSlot == null) return;

        _activeSlot.GetComponent<Image>().sprite = item.Item.icon;
    }
    #endregion
}
