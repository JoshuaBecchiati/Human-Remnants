using UnityEngine;

[CreateAssetMenu(fileName = "Melee", menuName = "ScriptableObjects/Melee", order = 1)]
public class Melee : Weapon, IMelee
{
    [Header("Melee Info")]
    public EMeleeType Type;                             // Tipi di arma
    [SerializeField] private float _attackDuration;     // Tempo tra un attacco e l’altro
    [SerializeField] private Collider _swordCol;

    public float AttackDuration => _attackDuration;
    public Collider SwordCol => _swordCol;
}
