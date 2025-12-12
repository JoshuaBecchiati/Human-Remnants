using UnityEngine;

public class Player : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Stats")]
    [SerializeField] private Characters m_character;
    [SerializeField] private float m_health = 100f;

    [Header("Combat settings")]
    [SerializeField] private GameObject _combatPF;

    // --- Proprierties ---
    public Characters Name => m_character;
    public float Health => m_health;
    public GameObject CombatPF => _combatPF;

    public void SetHealth(float health)
    {
        m_health = health;
    }
}
