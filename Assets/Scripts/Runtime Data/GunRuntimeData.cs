public class GunRuntimeData
{
    public int MagazineAmmo;
    public int CurrentAmmo;
    public float CurrentCooldown;

    public GunRuntimeData(int startingMagazineAmmo, int startingCurrentAmmo)
    {
        MagazineAmmo = startingMagazineAmmo;
        CurrentAmmo = startingCurrentAmmo;
        CurrentCooldown = 0f;
    }
}