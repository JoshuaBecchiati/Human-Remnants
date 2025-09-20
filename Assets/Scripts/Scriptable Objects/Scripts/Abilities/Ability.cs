using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    public string description;
    // public Sprite icon;
    public AbilityTypeDamage type;
    public int maxCharge;
}
