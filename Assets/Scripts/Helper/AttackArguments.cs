public class AttackArguments
{
    public UnitBase MainTarget { get; set; }
    public UnitBase[] OtherTargets { get; set; }
    public int Damage { get; set; }
    public int SplashDamage { get; set; }

}
