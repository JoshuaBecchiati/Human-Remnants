using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase
{
    [SerializeField] private List<AbilityData> _AbilitiesData = new();
    [SerializeField] private UIBattleManager m_uiBattleManager;

    public event Action OnPlayerDeath;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(UIBattleManager uIBattleManager)
    {
        m_uiBattleManager = uIBattleManager;
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

        if (_AbilitiesData != null)
            foreach (AbilityData ability in _AbilitiesData)
                ability.CharchingAbility();

        if (m_uiBattleManager != null)
            m_uiBattleManager.CreateAbilityUI(_AbilitiesData);
    }

    public IReadOnlyList<AbilityData> GetAbilities() => _AbilitiesData;
}
