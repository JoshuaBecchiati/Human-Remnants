using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject _ammoUI;
    [SerializeField] private TextMeshProUGUI _tmpAmmo;

    private void Awake()
    {
        PlayerCombat.OnWeaponEquipped += HandleWeaponEquipped;
        PlayerCombat.OnAmmoChanged += UpdateAmmoUI;
    }
    private void Start()
    {
        _ammoUI.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerCombat.OnWeaponEquipped -= HandleWeaponEquipped;
        PlayerCombat.OnAmmoChanged -= UpdateAmmoUI;
    }

    private void HandleWeaponEquipped(Ranged weapon)
    {
        _ammoUI.SetActive(true);
        UpdateAmmoUI(weapon.Magazine, weapon.MagazineSize);
    }

    private void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        _tmpAmmo.text = $"{currentAmmo}/{maxAmmo} - Ammo";
    }
}
