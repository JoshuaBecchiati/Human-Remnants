using UnityEngine;

[CreateAssetMenu(fileName = "AttackItem", menuName ="Items/AttackItem")]

public class AttackItem : Item
{
    [SerializeField] private float _damageAmount;

    public override void Use(UnitBase target)
    {
        target.TakeDamage(_damageAmount);
    }
}
