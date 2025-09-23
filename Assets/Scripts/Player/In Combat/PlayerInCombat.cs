using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase
{
    [SerializeField] private List<AbilityData> _AbilitiesData = new();
    [SerializeField] private UIBattleManager m_battleManager;

    public event Action OnPlayerDeath;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(UIBattleManager uIBattleManager)
    {
        m_battleManager = uIBattleManager;
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

        m_battleManager.CreateAbilityUI(_AbilitiesData);
    }

    public IReadOnlyList<AbilityData> GetAbilities() => _AbilitiesData;
}
