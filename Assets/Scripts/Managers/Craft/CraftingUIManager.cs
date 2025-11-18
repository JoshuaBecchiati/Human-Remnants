using UnityEngine;
using UnityEngine.UI;

public class CraftingUIManager : MonoBehaviour
{
    // --- Private ---
    private static CraftingSlot ActiveSlot;

    // --- Instance ---
    public static CraftingUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SelectSlot(CraftingSlot slot)
    {
        ActiveSlot = slot;
    }

    public static void PutItemInActiveSlot(ItemData item)
    {
        if (ActiveSlot == null) return;

        ActiveSlot.GetComponent<Image>().sprite = item.Item.icon;
    }
}
