using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInCombat : UnitBase
{
    public event Action OnEnemyDeath;

    protected override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(Health <= 0)
            OnEnemyDeath?.Invoke();
    }
}
