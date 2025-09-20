using UnityEngine;

[CreateAssetMenu(fileName = "AttackItem", menuName ="Items/AttackItem")]

public class AttackItem : Item
{
    [SerializeField] private float _damageAmount;

    public float DamageAmount => _damageAmount;
}
