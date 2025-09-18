using System;
public interface IEnemy
{
    int Score { get; }
    event Action<IEnemy> OnDeath;
}
