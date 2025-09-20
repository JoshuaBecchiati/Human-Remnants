using System;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int qty;


    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<Player>();
            if (player == null) return;

            //player.PickUpWeapon(_weapon);
            Destroy(gameObject);
        }
    }
}
