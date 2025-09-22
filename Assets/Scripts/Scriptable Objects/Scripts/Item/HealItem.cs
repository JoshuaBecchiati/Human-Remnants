using UnityEngine;

[CreateAssetMenu(fileName ="HealItem", menuName ="Items/HealItem")]

public class HealItem : Item
{
    [SerializeField] private float _healAmount;

    public override void UseItem(UnitBase target)
    {
        target.Heal(_healAmount);
    }
}
