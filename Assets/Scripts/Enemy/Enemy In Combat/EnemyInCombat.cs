using System.Collections.Generic;
using UnityEngine;

public class EnemyInCombat : UnitBase
{
    public AttackData GetAttack()
    {
        int total = 0;

        foreach (AttackData attack in _attacks)
            total += attack.Possibility;

        int roll = Random.Range(0, total);

        int current = 0;

        foreach (AttackData attack in _attacks)
        {
            current += attack.Possibility;

            if (roll < current)
                return attack;
        }

        return _attacks[_attacks.Count - 1];
    }
}
