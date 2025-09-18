using UnityEngine;

[CreateAssetMenu(fileName = "Ranged", menuName = "ScriptableObjects/Ranged", order = 0)]
public class Ranged : Weapon, IRanged
{
    [Header("Ranged Info")]
    public EGunType Type;                               // Tipo di arma
    [SerializeField] private GameObject _projectile;    // Prefab del proiettile

    [Header("Stats")]
    [SerializeField] private float _forceProjectile;    // Forza del proiettile
    [SerializeField] private float _reloadTime;         // Tempo di ricarica
    [SerializeField] private float _fireRate;           // Colpi al secondo
    [SerializeField] private int _magazineSize;         // Caricatore
    [SerializeField] private int _magazine;


    public GameObject Projectile => _projectile;
    public float ForceProjectile => _forceProjectile;
    public float ReloadTime => _reloadTime;
    public float FireRate => _fireRate;
    public int MagazineSize => _magazineSize;
    public int Magazine => _magazine;
}
