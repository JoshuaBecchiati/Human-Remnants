using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpScroll : SaveableObject
{
    [SerializeField] private Ability m_scroll;
    [SerializeField] private TextMeshProUGUI m_textInfoPickUp;

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.parent.CompareTag("Player"))
        {
            m_textInfoPickUp.gameObject.SetActive(true);
            m_textInfoPickUp.text = $"Press [E] to learn {m_scroll.name}";

            PlayerInputSingleton.Instance.Actions["Interact"].performed += PickUp;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.parent.CompareTag("Player"))
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
        AbilityManager.Instance.AddAbility(m_scroll);
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
