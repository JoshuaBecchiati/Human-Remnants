using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Stats")]
    [SerializeField] private Characters m_character;
    [SerializeField] private float m_health = 100f;

    [Header("Combat settings")]
    [SerializeField] private GameObject _combatPF;
    [SerializeField] private List<AbilityData> m_abilites = new();

    // --- Proprierties ---
    public Characters Name => m_character;
    public float Health => m_health;
    public GameObject CombatPF => _combatPF;

    public void SetHealth(float health)
    {
        m_health = health;
    }

    public IReadOnlyList<AbilityData> GetAbilities()
    {
        return m_abilites;
    }
}
