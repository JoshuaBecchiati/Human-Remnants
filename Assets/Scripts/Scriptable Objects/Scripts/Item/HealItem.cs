using UnityEngine;

[CreateAssetMenu(fileName ="HealItem", menuName ="Items/HealItem")]

public class HealItem : Item
{
    [SerializeField] private float _healAmount;

    public override void Use(UnitBase target)
    {
        target.Heal(_healAmount);
    }
}
