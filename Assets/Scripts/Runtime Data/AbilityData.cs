[System.Serializable]
public class AbilityData
{
    public Ability ability;
    public int chargeCounter;

    public AbilityData(Ability ability)
    {
        this.ability = ability;
        chargeCounter += ability.maxCharge;
    }

    public void CharchingAbility() => chargeCounter++;
}
