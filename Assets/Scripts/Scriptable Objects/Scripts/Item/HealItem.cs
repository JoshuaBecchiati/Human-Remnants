using UnityEngine;

[CreateAssetMenu(fileName ="HealItem", menuName ="Items/HealItem")]

public class HealItem : Item
{
    [SerializeField] private float _healAmount;

    public float HealAmount => _healAmount;
}
