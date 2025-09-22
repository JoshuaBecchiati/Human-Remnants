using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase
{
    [SerializeField] private List<AbilityData> _AbilitiesData = new();

    public event Action OnPlayerDeath;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (Health <= 0)
            OnPlayerDeath?.Invoke();
    }

    public override void StartTurn()
    {
        base.StartTurn();
        foreach (AbilityData ability in _AbilitiesData)
            ability.CharchingAbility();
    }

    public IReadOnlyList<AbilityData> GetAbilities() => _AbilitiesData;
}
