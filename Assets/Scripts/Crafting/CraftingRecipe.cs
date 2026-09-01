using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crafting recipe", menuName = "Crafting recipe")]

public class CraftingRecipe : ScriptableObject
{
    public string recipeName = "New recipe";
    public List<ItemData> ingridients;
    public Item result;
    public int resultAmount = 1;
}
