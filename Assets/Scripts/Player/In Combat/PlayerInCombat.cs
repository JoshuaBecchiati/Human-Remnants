using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase, IPlayer
{
    public event Action OnPlayerDeath;

    protected override void Awake()
    {
        base.Awake();
        BattleManager.Instance.OnUseItem += UseItem;
    }
    private void OnDestroy()
    {
        BattleManager.Instance.OnUseItem -= UseItem;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (Health <= 0)
            OnPlayerDeath?.Invoke();
    }

    private void UseItem(UnitBase target, ItemData item)
    {
        _target = target;

        if (item.item is HealItem healItem)
            Heal(healItem.HealAmount);
        else if (item.item is AttackItem attackItem)
            Attack(attackItem.DamageAmount);
        InventoryManager.Instance.RemoveItem(item);
    }

    public void Attack(float damage)
    {
        Debug.Log($"{_name} has attacked {_target.name}. Damage inflicted: {damage}.");
        _target.TakeDamage(damage);

        _target = null;
    }
}
