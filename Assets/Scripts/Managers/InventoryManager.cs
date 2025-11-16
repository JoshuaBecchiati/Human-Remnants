using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    [SerializeField] private static List<ItemData> _itemsData = new();

    [SerializeField] private NewBattleManager m_battleManager;

    // --- Instance ---
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

    private void Start()
    {
        if (m_battleManager != null)
            m_battleManager.OnUseItem += RemoveItemInInventory;
    }
    private void OnDestroy()
    {
        if (m_battleManager != null)
            m_battleManager.OnUseItem -= RemoveItemInInventory;
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
        OnRemoveItem?.Invoke();
    }

    /// <summary>
    /// Remove item for crafting
    /// </summary>
    /// <param name="item"></param>
    /// <param name="qty"></param>
    public void RemoveItemInInventory(ItemData item, int qty)
    {
        int index = _itemsData.FindIndex(i => i == item);
        Debug.Log("ITEM: " + _itemsData[index].Item);
        _itemsData[index].RemoveItem(qty);

        if(_itemsData[index].Qty <= 0)
            _itemsData.RemoveAt(index);
        OnRemoveItem?.Invoke();
    }

    public static ItemData FindItemByName(string name)
    {
        foreach (ItemData itemData in _itemsData)
        {
            if (name == itemData.Item.name)
            {
                return itemData;
            }
        }

        return null;
    }

    public IReadOnlyList<ItemData> GetItems()
    {
        return _itemsData;
    }
}
