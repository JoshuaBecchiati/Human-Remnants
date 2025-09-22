using UnityEngine;

[CreateAssetMenu(fileName = "AttackItem", menuName ="Items/AttackItem")]

public class AttackItem : Item
{
    [SerializeField] private float _damageAmount;

    public override void UseItem(UnitBase target)
    {
        target.TakeDamage(_damageAmount);
    }
}
