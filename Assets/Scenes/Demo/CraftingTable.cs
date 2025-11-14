using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class CraftingTable : CraftManager
{
    [SerializeField] private CraftingRecipe m_recipe;
    [SerializeField] private GameObject m_craftMenu;
    [SerializeField] private GameObject m_infoText;
    [SerializeField] private TextMeshProUGUI m_infoLabel;

    private bool _playerNearby;

    public event Action OnOpenCrafting;
    public event Action OnCloseCrafting;

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

        PlayerInputSingleton.Instance.Actions["Interact"].performed += OnInteract;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;
        m_infoText.SetActive(false);
        m_craftMenu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        PlayerInputSingleton.Instance.Actions["Interact"].performed -= OnInteract;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!_playerNearby) return; // sicurezza in più

        if (m_craftMenu.activeSelf)
        {
            m_craftMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            OnOpenCrafting?.Invoke();
        }
        else
        {
            m_craftMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            OnCloseCrafting?.Invoke();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
            UpdateInfoText();
    }

    public void Craft()
    {
        Craft(m_recipe);
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
}
