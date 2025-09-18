using System.Collections;
using UnityEngine;

public class EnemyRanged : Enemy, IDamageDealer, IDamageable
{
    [Header("Weapon")]
    [SerializeField] private Ranged _equippedWeapon;
    [SerializeField] private Transform _hand;
    [SerializeField] private GameObject _weaponInstance;

    [Header("Attack")]
    [SerializeField] private bool _isAlreadyAttacked;
    [SerializeField] private bool _isReloading;

    private GunRuntimeData _currentGunData;

    public float Damage => _equippedWeapon.Damage;

    private void Awake()
    {
        EquipWeapon(_equippedWeapon);
    }

    private void Update()
    {
        Movement();

        // Aggiorna cooldown ogni frame
        if (_currentGunData.CurrentCooldown > 0f)
            _currentGunData.CurrentCooldown -= Time.deltaTime;

        if (IsAttacking() && !_isReloading) Shoot();
    }

    private void EquipWeapon(Ranged weapon)
    {
        _equippedWeapon = weapon;
        _currentGunData = new GunRuntimeData(weapon.MagazineSize, weapon.Magazine);

        if (_weaponInstance != null)
            Destroy(_weaponInstance);

        // Crea il modello nell’hand slot
        _weaponInstance = Instantiate(weapon.Prefab, _hand);
    }

    public void Shoot()
    {
        transform.LookAt(Player);
        if (_currentGunData.CurrentAmmo > 0 && _currentGunData.CurrentCooldown <= 0f)
        {
            _currentGunData.CurrentAmmo--;
            _currentGunData.CurrentCooldown = 1f / _equippedWeapon.FireRate;

            Transform shootPoint = _weaponInstance.transform.Find("Shoot_Point");

            var proj = Instantiate(_equippedWeapon.Projectile, shootPoint.position, shootPoint.rotation).GetComponent<Projectile>();
            proj.Init(this);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.AddForce(shootPoint.forward * _equippedWeapon.ForceProjectile, ForceMode.Impulse);
        }
        else if (_currentGunData.CurrentAmmo <= 0 && !_isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(_equippedWeapon.ReloadTime);
        _currentGunData.CurrentAmmo = _equippedWeapon.MagazineSize;
        _isReloading = false;
    }
}