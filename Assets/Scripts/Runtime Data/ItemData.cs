using UnityEngine;

public class ItemData : MonoBehaviour
{
    public Item item;
    public int qty;

    public ItemData(Item item)
    {
        this.item = item;
        qty++;
    }
}
