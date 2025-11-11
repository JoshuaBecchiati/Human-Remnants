using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTable : CraftManager
{
    [SerializeField] private CraftingRecipe recipe;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Craft(recipe);
    }

}
