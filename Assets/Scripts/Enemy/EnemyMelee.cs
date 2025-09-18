using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy, IDamageDealer, IDamageable
{
    [Header("Weapon")]
    [SerializeField] private Melee _equippedWeapon;
    [SerializeField] private Transform _hand;
    [SerializeField] private GameObject _weaponInstance;

    [Header("Attack")]
    [SerializeField] private bool _isAttackRange;

    public float Damage => _equippedWeapon.Damage;

    private void Awake()
    {
        EquipWeapon(_equippedWeapon);
    }

    private void Update()
    {
        Movement();
        if (IsAttacking())
        {
            Agent.updateRotation = true;
            StartCoroutine(SwingRoutine(_equippedWeapon));
        }
    }

    private void EquipWeapon(Melee weapon)
    {
        _equippedWeapon = weapon;

        if (_weaponInstance != null)
            Destroy(_weaponInstance);

        // Istanzia prefab e prendi la spada
        _weaponInstance = Instantiate(weapon.Prefab, _hand);
        _weaponInstance.GetComponent<Sword>().Init(this);
        _weaponInstance.GetComponent<Collider>().enabled = false;
    }

    private IEnumerator SwingRoutine(Melee weapon)
    {
        var col = _weaponInstance.GetComponent<Collider>();
        col.enabled = true;
        yield return new WaitForSeconds(weapon.AttackDuration);
        col.enabled = false;
    }
}
