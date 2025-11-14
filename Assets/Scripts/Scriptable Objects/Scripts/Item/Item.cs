using UnityEngine;

[CreateAssetMenu(fileName = "Generic item", menuName = "Items/Generic item")]

public class Item : ScriptableObject
{
    public new string name;
    public string description;
    // public Sprite icon;
    public ItemType type;

    public virtual void Use(UnitBase target)
    {
        Debug.LogError("[Item] Use method not implemented"); 
    }
}
