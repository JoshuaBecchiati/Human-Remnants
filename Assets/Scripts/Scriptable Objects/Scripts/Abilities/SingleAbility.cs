using UnityEngine;

[CreateAssetMenu(fileName = "Single ability", menuName = "Battle/Abilities/Single ability")]

public class SingleAbility : Ability
{

    public override void Use(UnitBase[] targets)
    {
        switch (effectType)
        {
            case AbilityEffectType.Damage:
                {
                    targets[0].TakeDamage(effectValue);
                }
                break;
            case AbilityEffectType.Heal:
                {
                    targets[0].Heal(effectValue);
                }
                break;
            case AbilityEffectType.Buff:
                break;
            case AbilityEffectType.Debuff:
                break;
        }
    }
}
