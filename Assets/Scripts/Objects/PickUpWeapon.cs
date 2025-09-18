using System;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;


    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            var player = col.GetComponent<PlayerCombat>();
            if (player == null) return;

            player.PickUpWeapon(_weapon);
            Destroy(gameObject);
        }
    }
}
