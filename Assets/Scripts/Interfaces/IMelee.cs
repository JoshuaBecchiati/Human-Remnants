using UnityEngine;

public interface IMelee
{
    float AttackDuration { get; }
    Collider SwordCol { get; }
}