using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] private List<ItemData> _itemsData = new();

    [SerializeField] private BattleManager _battleManager;

    // --- Static ---
    public static InventoryManager Instance { get; private set; }

    // --- Proprieties ---
    public event Action OnAddItem;
    public event Action<ItemData> OnRemoveItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (_battleManager != null)
            _battleManager.OnUseItem += RemoveItemInInventory;
    }
    private void OnDestroy()
    {
        if (_battleManager != null)
            _battleManager.OnUseItem -= RemoveItemInInventory;
    }

    public void AddItemInInventory(Item item, int qty)
    {
        if (_itemsData.Exists(i => i.Item == item))
        {
            int index = _itemsData.FindIndex(i => i.Item == item);
            _itemsData[index].AddItem(qty);
        }
        else
        {
            ItemData newItem = new(item, qty);
            _itemsData.Add(newItem);
        }
        OnAddItem?.Invoke();
    }

    /// <summary>
    /// Remove item in combat
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItemInInventory(ItemData item)
    {
        int index = _itemsData.FindIndex(i => i == item);
        _itemsData[index].RemoveItem();
        if (_itemsData[index].Qty <= 0)
            _itemsData.RemoveAt(index);
        OnRemoveItem?.Invoke(item);
    }

    /// <summary>
    /// Remove item for crafting
    /// </summary>
    /// <param name="item"></param>
    /// <param name="qty"></param>
    public void RemoveItemInInventory(ItemData item, int qty)
    {
        int index = _itemsData.FindIndex(i => i == item);
        _itemsData[index].RemoveItem(qty);

        if(_itemsData[index].Qty <= 0)
            _itemsData.RemoveAt(index);
        OnRemoveItem?.Invoke(item);
    }


    public IReadOnlyList<ItemData> GetItems() => _itemsData;
}
