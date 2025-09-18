using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerCombat : MonoBehaviour, IDamageDealer
{
    [SerializeField] private Transform rightHandSlot;
    [SerializeField] private Weapon _selectedWeapon;
    [SerializeField] private List<Weapon> _equippedWeapons;
    [SerializeField] private GameObject _weaponInstance;
    [SerializeField] private bool _isRealoading;

    private GunRuntimeData _currentGunData;
    public float Damage => _selectedWeapon.Damage;

    public static event Action<int, int> OnAmmoChanged; // currentAmmo, maxAmmo
    public static event Action<Ranged> OnWeaponEquipped;

    private void Awake()
    {
        Ammo.OnPickUpAmmo += PickUpAmmo;
    }

    private void Update()
    {
        // Aggiorna sempre cooldown
        if (_selectedWeapon is Ranged ranged && _currentGunData.CurrentCooldown > 0f)
            _currentGunData.CurrentCooldown -= Time.deltaTime;

        // Controllo ricarica
        if (Input.GetKeyDown(KeyCode.R) && _selectedWeapon is Ranged reloadWeapon && !_isRealoading)
            if (_currentGunData.MagazineAmmo > 0) StartCoroutine(Reload(reloadWeapon));

        // Controllo sparo
        if (Mouse.current.leftButton.isPressed && _selectedWeapon != null && !_isRealoading && _selectedWeapon is Ranged rangedWeapon) Shoot(rangedWeapon);
        if (Input.GetKeyDown(KeyCode.Mouse0) && _selectedWeapon is Melee meleeWeapon) StartCoroutine(SwingRoutine(meleeWeapon));

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedWeapon = _equippedWeapons[0];
            PickUpWeapon(_selectedWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedWeapon = _equippedWeapons[1];
            PickUpWeapon(_selectedWeapon);
        }
    }

    /*
     * 
     * [Equip Handling]
     * 
     */

    // Gestione interna dell’equipaggiamento
    private void EquipWeapon(Ranged weapon)
    {
        _selectedWeapon = weapon;
        if (!_equippedWeapons.Contains(weapon))
        {
            _equippedWeapons.Add(weapon);
            _currentGunData = new GunRuntimeData(weapon.MagazineSize, weapon.Magazine);
        }

        if (_weaponInstance != null)
            Destroy(_weaponInstance);

        // Crea il modello nell’hand slot
        _weaponInstance = Instantiate(weapon.Prefab, rightHandSlot);

        OnAmmoChanged?.Invoke(_currentGunData.CurrentAmmo, _currentGunData.MagazineAmmo);
        OnWeaponEquipped?.Invoke(weapon);
    }

    private void EquipWeapon(Melee weapon)
    {
        _selectedWeapon = weapon;

        if(!_equippedWeapons.Contains(weapon)) _equippedWeapons.Add(weapon);

        if (_weaponInstance != null)
            Destroy(_weaponInstance);

        // Istanzia prefab e prendi la spada
        _weaponInstance = Instantiate(weapon.Prefab, rightHandSlot);
        _weaponInstance.GetComponent<Sword>().Init(this);
        _weaponInstance.GetComponent<Collider>().enabled = false;
    }

    // Metodo pubblico che i pickup chiamano
    public void PickUpWeapon(Weapon weapon)
    {
        if (weapon is Ranged rangedWeapon) EquipWeapon(rangedWeapon);
        else if (weapon is Melee meleeWeapon) EquipWeapon(meleeWeapon);
    }

    private void PickUpAmmo(int ammo)
    {
        _currentGunData.MagazineAmmo += ammo;
        OnAmmoChanged?.Invoke(_currentGunData.CurrentAmmo, _currentGunData.MagazineAmmo);
    }

    /*
     * 
     * [Attack Handling]
     * 
     */

    private void Shoot(Ranged weapon)
    {
        // Sparo
        if (_currentGunData.CurrentAmmo > 0 && _currentGunData.CurrentCooldown <= 0f)
        {
            _currentGunData.CurrentAmmo--;
            OnAmmoChanged?.Invoke(_currentGunData.CurrentAmmo, _currentGunData.MagazineAmmo);

            _currentGunData.CurrentCooldown = 1f / weapon.FireRate;

            Transform shootPoint = _weaponInstance.transform.Find("Shoot_Point");
            var proj = Instantiate(weapon.Projectile, shootPoint.position, shootPoint.rotation).GetComponent<Projectile>();
            proj.Init(this);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.AddForce(shootPoint.forward * weapon.ForceProjectile, ForceMode.Impulse);
        }
        else if (_currentGunData.CurrentAmmo <= 0 && _currentGunData.MagazineAmmo > 0)
        {
            StartCoroutine(Reload(weapon));
        }
    }
    private IEnumerator Reload(Ranged weapon)
    {
        _isRealoading = true;
        yield return new WaitForSeconds(weapon.ReloadTime);

        int missing = weapon.MagazineSize - _currentGunData.CurrentAmmo;
        int ammoToLoad = Mathf.Min(missing, _currentGunData.MagazineAmmo);

        _currentGunData.CurrentAmmo += ammoToLoad;
        _currentGunData.MagazineAmmo -= ammoToLoad;

        _isRealoading = false;
        OnAmmoChanged?.Invoke(_currentGunData.CurrentAmmo, _currentGunData.MagazineAmmo);
    }


    private IEnumerator SwingRoutine(Melee weapon)
    {
        var col = _weaponInstance.GetComponent<Collider>();
        col.enabled = true;
        yield return new WaitForSeconds(weapon.AttackDuration);
        col.enabled = false;
    }
}
