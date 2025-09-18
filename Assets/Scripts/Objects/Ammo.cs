using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] int _ammo;
    public static event Action<int> OnPickUpAmmo;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            OnPickUpAmmo?.Invoke(_ammo);
            Destroy(gameObject);
        }
    }
}
