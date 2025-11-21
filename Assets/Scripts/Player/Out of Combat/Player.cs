using UnityEngine;

public class Player : MonoBehaviour
{
    // --- Inspector References ---
    [Header("Stats")]
    [SerializeField] private string m_name = string.Empty;
    [SerializeField] private float m_health = 100f;

    [Header("Combat settings")]
    [SerializeField] private GameObject _combatPF;

    // --- Proprierties ---
    public string Name => m_name;
    public float Health => m_health;
    public GameObject CombatPF => _combatPF;

    public void SetHealth(float health)
    {
        m_health = health;
    }
}
