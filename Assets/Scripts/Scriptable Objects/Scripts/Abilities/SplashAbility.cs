using UnityEngine;

[CreateAssetMenu(fileName = "Splash ability", menuName = "Abilities/Splash ability")]

public class SplashAbility : Ability
{
    public float splashEffectValue;
    public AbilitySplashTargets otherTargets;

    public override void Use(UnitBase[] targets)
    {
        int lenght = 1;
        if (targets.Length > 1)
            lenght = targets.Length - (int)otherTargets;

        switch (effectType)
        {
            case AbilityEffectType.Damage:
                {
                    targets[0].TakeDamage(effectValue);
                    for (int i = 1; i < lenght; i++)
                        targets[i].TakeDamage(splashEffectValue);
                }
                break;
            case AbilityEffectType.Heal:
                {
                    targets[0].Heal(effectValue);
                    for (int i = 1; i < lenght; i++)
                        targets[i].Heal(splashEffectValue);
                }
                break;
            case AbilityEffectType.Buff:
                break;
            case AbilityEffectType.Debuff:
                break;
        }
    }
}
