[System.Serializable]
public class ItemData
{
    public Item item;
    public int qty;

    public ItemData(Item item, int qty)
    {
        this.item = item;
        this.qty += qty;
    }
}
