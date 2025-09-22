using System;
using System.Diagnostics;

[Serializable]
public class ItemData
{
    public Item Item { get; private set; }
    public int Qty { get; private set; }


    public ItemData(Item item, int qty)
    {
        Item = item;
        Qty += qty;
    }

    public void AddItem(int qty)
    {
        if (qty > 0)
            Qty += qty;
        else
            Debug.WriteLine("Quantity can't be zero or a negative number");
    }

    public void RemoveItem()
    {
        if (Qty - 1 >= 0)
            Qty -= 1;
        else
            Debug.WriteLine("You can't go under 0");
    }

    public void RemoveItem(int qty)
    {
        if (qty > 0)
        {
            if (Qty - qty >= 0)
                Qty -= qty;
            else
                Debug.WriteLine("You can't go under 0");
        }
        else
        {
            Debug.WriteLine("Quantity can't be zero or a negative number");
        }
    }

    public void UseItem(UnitBase target)
    {
        Item.UseItem(target);
    }
}
