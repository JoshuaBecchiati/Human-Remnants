using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventorySave : SaveableObject
{
    private List<ItemData> _inventory = new List<ItemData>();

    public override void SaveState(SaveData save)
    {
        if (InventoryManager.Instance != null)
            _inventory = InventoryManager.Instance.GetItems().ToList();

        if (save.inventory != null)
            save.inventory = _inventory;
    }

    public override void LoadState(SaveData save)
    {
        if (save.inventory != null)
            InventoryManager.Instance.SetItems(save.inventory);
    }
}
