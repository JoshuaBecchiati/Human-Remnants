using UnityEngine;

public abstract class Item : ScriptableObject
{
    public new string name;
    public string description;
    // public Sprite icon;
    public ItemType type;

    public abstract void UseItem(UnitBase target);
}
