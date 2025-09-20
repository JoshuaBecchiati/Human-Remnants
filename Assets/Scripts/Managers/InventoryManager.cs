using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private List<Item> _items = new();

    private List<ItemData> _itemsData = new();

    // --- Static ---
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        AddItem();
    }

    private void AddItem()
    {
        foreach (Item item in _items)
            _itemsData.Add(new(item));
    }

    public IReadOnlyList<ItemData> GetItems() => _itemsData;
}
