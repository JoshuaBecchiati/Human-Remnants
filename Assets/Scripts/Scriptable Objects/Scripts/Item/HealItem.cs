using UnityEngine;

[CreateAssetMenu(fileName ="HealItem", menuName ="Items/HealItem")]

public class HealItem : Item
{
    [SerializeField] private float _healAmount;
    [SerializeField] private GameObject _healVFXPrefab;

    public override void Use(UnitBase target)
    {
        target.Heal(_healAmount);
        if(_healVFXPrefab != null)
        {
            GameObject vfx = GameObject.Instantiate(_healVFXPrefab, target.transform.position, _healVFXPrefab.transform.rotation);
            vfx.transform.SetParent(target.transform);
            GameObject.Destroy(vfx, 3f);
        }
            
    }
}
