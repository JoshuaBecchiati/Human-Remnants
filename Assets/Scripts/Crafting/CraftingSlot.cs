using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : MonoBehaviour
{
    public ItemData Item;

    public void SetItemInSlot(ItemData item)
    {
        Item = item;
    }
}
