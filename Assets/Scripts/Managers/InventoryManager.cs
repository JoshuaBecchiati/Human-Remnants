using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<ItemData> _itemsData = new();

    // --- Static ---
    public static InventoryManager Instance { get; private set; }

    // --- Proprieties ---
    public event Action OnAddItem;
    public event Action OnRemoveItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(Item item, int qty)
    {
        if (_itemsData.Exists(i => i.item == item))
        {
            int index = _itemsData.FindIndex(i => i.item == item);
            _itemsData[index].qty += qty;
        }
        else
        {
            _itemsData.Add(new(item, qty));
        }
        OnAddItem?.Invoke();
    }

    /// <summary>
    /// Remove item in combat
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(ItemData item)
    {
        int index = _itemsData.FindIndex(i => i == item);
        _itemsData[index].qty -= 1;
        if (_itemsData[index].qty <= 0)
            _itemsData.RemoveAt(index);
        UIBattleManager.Instance.UpdateUI();
        OnRemoveItem?.Invoke();
    }

    /// <summary>
    /// Remove item for crafting
    /// </summary>
    /// <param name="item"></param>
    /// <param name="qty"></param>
    public void RemoveItem(ItemData item, int qty)
    {
        int index = _itemsData.FindIndex(i => i == item);
        _itemsData[index].qty -= qty;
        if(_itemsData[index].qty <= 0)
            _itemsData.RemoveAt(index);
        OnRemoveItem?.Invoke();
    }


    public IReadOnlyList<ItemData> GetItems() => _itemsData;
}
