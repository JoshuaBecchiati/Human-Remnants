using UnityEngine;

public class Weapon : ScriptableObject
{
    [Header("Common Info")]
    public string Name;           // Nome arma
    public GameObject Prefab;     // Prefab del modello
    public float Damage;          // Danno base

    public EWeaponType WeaponType; // Enum: Melee / Ranged
}
