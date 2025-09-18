using UnityEngine;

public interface IRanged
{
    int Magazine {  get; }
    int MagazineSize { get; }
    float ReloadTime { get; }
    float FireRate { get; }
    GameObject Projectile { get; }
    float ForceProjectile { get; }
}
