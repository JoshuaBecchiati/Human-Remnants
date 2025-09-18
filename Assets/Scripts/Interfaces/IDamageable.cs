public interface IDamageable
{
    void TakeDamage(float amount);
}

public interface IDamageDealer
{
    float Damage { get; }
}
