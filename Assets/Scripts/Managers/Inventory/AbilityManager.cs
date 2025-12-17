using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private List<AbilityData> m_abilitesData = new();

    [SerializeField] private BattleManager m_battleManager;

    // --- Instance ---
    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddAbility(Ability ability)
    {
        AbilityData abilityData = new(ability);
        m_abilitesData.Add(abilityData);
    }

    public AbilityData FindAbilityByName(string name)
    {
        foreach (AbilityData abilityData in m_abilitesData)
        {
            if (name == abilityData.Ability.name)
            {
                return abilityData;
            }
        }

        return null;
    }

    public void SetAbilities(List<AbilityData> abilities)
    {
        if (abilities == null) return;

        m_abilitesData = abilities;
    }

    public IReadOnlyList<AbilityData> GetAbilites()
    {
        return m_abilitesData;
    }
}
