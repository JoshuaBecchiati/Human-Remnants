using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField] private List<CraftingRecipe> recipes;

    public static CraftManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region Craft by item
    public bool Craft(ItemData[] items)
    {
        CraftingRecipe recipe = FindRecipeFromItems(items);

        if (recipe != null)
        {
            Craft(recipe);
            return true;
        }

        return false;
    }

    private CraftingRecipe FindRecipeFromItems(ItemData[] items)
    {
        foreach (var recipe in recipes)
        {
            if (RecipeMatchesItems(recipe, items))
                return recipe;
        }

        return null;
    }

    private bool RecipeMatchesItems(CraftingRecipe recipe, ItemData[] items)
    {
        // Se il numero degli ingredienti non coincide, già non è la ricetta giusta
        if (recipe.ingridients.Count != items.Length)
            return false;

        foreach (var ingredient in recipe.ingridients)
        {
            bool foundMatch = false;

            foreach (var item in items)
            {
                if (item != null &&
                    item.Item == ingredient.Item &&
                    item.Qty >= ingredient.Qty)
                {
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
                return false;
        }

        return true;
    }
    #endregion

    #region Craft by recipe
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
            foreach (ItemData item in InventoryManager.Instance.GetItems())
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
    #endregion

    private void ConsumeIngridients(CraftingRecipe recipe)
    {
        foreach (ItemData ingridients in recipe.ingridients)
        {
            foreach (ItemData item in new List<ItemData>(InventoryManager.Instance.GetItems()))
            {
                if (item.Item == ingridients.Item)
                    InventoryManager.Instance.RemoveItemInInventory(item, ingridients.Qty);
            }
        }
    }

    private void CreateResult(CraftingRecipe recipe)
    {
        InventoryManager.Instance.AddItemInInventory(recipe.result, recipe.resultAmount);
        Debug.Log($"{recipe.result.name} added to inventory. Crafted amount: {recipe.resultAmount}");
    }
}
