using System;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private Item m_item;
    [SerializeField] private int m_qty = 1;
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItemInInventory(m_item, m_qty);
            Destroy(gameObject);
        }
    }
}