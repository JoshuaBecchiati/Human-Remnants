using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    // --- Inspector ---
    [SerializeField] private Attack m_attack;

    // --- Public ---
    public string Name => m_attack.name;
    public float Damage => m_attack.Damage;
    public int Possibility => m_attack.Possibility;
    public GameObject Animation => m_attack.Animation;
}
