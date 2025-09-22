using System;

[Serializable]
public class AbilityData
{
    public Ability Ability;
    public int ChargeCounter;

    public AbilityData(Ability ability)
    {
        Ability = ability;
        ChargeCounter = 0;
    }

    public void UseAbility(UnitBase[] targets)
    {
        Ability.Use(targets);
        ChargeCounter = 0;
    }

    public void CharchingAbility()
    {
        if (ChargeCounter != Ability.maxCharge)
            ChargeCounter++;
    }

}
