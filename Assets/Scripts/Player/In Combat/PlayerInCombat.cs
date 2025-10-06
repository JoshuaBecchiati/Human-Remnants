using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase
{
    [SerializeField] private List<AbilityData> _AbilitiesData = new();
    
    private UIBattleManager m_uiBattleManager;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(UIBattleManager uIBattleManager)
    {
        m_uiBattleManager = uIBattleManager;
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
