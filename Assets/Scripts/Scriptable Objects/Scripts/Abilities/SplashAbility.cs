using UnityEngine;

[CreateAssetMenu(fileName = "Splash ability", menuName = "Abilities/Splash ability")]

public class SplashAbility : Ability
{
    public float splashEffectValue;

    public override void Use(UnitBase[] targets)
    {
        switch (effectType)
        {
            case AbilityEffectType.Damage:
                {
                    targets[0].TakeDamage(effectValue);
                    for (int i = 1; i < targets.Length; i++)
                        targets[i].TakeDamage(splashEffectValue);
                }
                break;
            case AbilityEffectType.Heal:
                {
                    targets[0].Heal(effectValue);
                    for (int i = 1; i < targets.Length; i++)
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
