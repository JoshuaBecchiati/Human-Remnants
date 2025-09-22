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
        {
            ability.CharchingAbility();
            Debug.Log("Charge: " + ability.ChargeCounter);
        }

        if (UIBattleManager.Instance == null)
            Debug.Log("Manager è null");
        UIBattleManager.Instance.CreateAbilityUI(_AbilitiesData);

    }

    public IReadOnlyList<AbilityData> GetAbilities() => _AbilitiesData;
}
