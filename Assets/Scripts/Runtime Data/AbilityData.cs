using System;
using UnityEngine;

[Serializable]
public class AbilityData
{
    [SerializeField] private Ability m_ability;
    [SerializeField] private int m_chargeCounter;

    public Ability Ability => m_ability;
    public int ChargeCounter => m_chargeCounter;

    public AbilityData(Ability ability)
    {
        m_ability = ability;
        m_chargeCounter = 0;
    }

    public void UseAbility(UnitBase[] targets)
    {
        m_ability.Use(targets);
        m_chargeCounter = 0;
    }

    public void CharchingAbility()
    {
        if (ChargeCounter != Ability.maxCharge)
            m_chargeCounter++;
    }

}
