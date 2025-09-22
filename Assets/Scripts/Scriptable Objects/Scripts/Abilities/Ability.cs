using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public new string name;
    public string description;
    public int maxCharge;
    public float effectValue;
    public AbilityTypeDamage damageType;
    public AbilityEffectType effectType;

    public abstract void Use(UnitBase[] targets);

    // public Sprite icon;
}
