using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> recipes;

    [Header("Dependencies")]
    [SerializeField] private InventoryManager m_inventory;

    public void Craft(CraftingRecipe recipe)
    {
        if (IsCraftable(recipe))
        {
            ConsumeIngridients(recipe);
            CreateResult(recipe);
        }
        else
        {
            Debug.Log("Can't craft");
        }
    }

    private bool IsCraftable(CraftingRecipe recipe)
    {
        foreach (ItemData ingridients in recipe.ingridients)
        {
            int itemCount = 0;
            foreach (ItemData item in m_inventory.GetItems())
            {
                Debug.Log($"[ITEM] {item.Item.name} - [QUANTITY] {item.Qty}");
                if (item.Item == ingridients.Item)
                {
                    itemCount = item.Qty;
                }
            }
            if (itemCount < ingridients.Qty)
            {
                return false;
            }
        }
        return true;
    }

    private void ConsumeIngridients(CraftingRecipe recipe)
    {
        foreach (ItemData ingridients in recipe.ingridients)
        {
            foreach (ItemData item in m_inventory.GetItems())
            {
                if (item.Item == ingridients.Item)
                    item.RemoveItem(ingridients.Qty);
            }
        }
    }

    private void CreateResult(CraftingRecipe recipe)
    {
        m_inventory.AddItemInInventory(recipe.result, recipe.resultAmount);
        Debug.Log($"{recipe.result.name} added to inventory. Crafted amount: {recipe.resultAmount}");
    }
}
