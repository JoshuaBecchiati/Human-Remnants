using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase, IPlayer
{
    public event Action OnPlayerDeath;

    protected override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (Health <= 0)
            OnPlayerDeath?.Invoke();
    }
}
