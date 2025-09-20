using System;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int qty = 1;


    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            InventoryManager.Instance.AddItem(item, qty);
            Destroy(gameObject);
        }
    }
}
