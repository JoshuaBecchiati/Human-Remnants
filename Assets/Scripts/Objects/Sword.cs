using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private IDamageDealer _owner;

    public void Init(IDamageDealer owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_owner != null && col.TryGetComponent<IDamageable>(out var h))
        {
            if (_owner is MonoBehaviour ownerMb && ownerMb.CompareTag(col.transform.tag))
                return;
            h.TakeDamage(_owner.Damage);
        }
    }
}
