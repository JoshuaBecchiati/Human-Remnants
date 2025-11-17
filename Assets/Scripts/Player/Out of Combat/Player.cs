using UnityEngine;

public class Player : CharController
{
    // --- Inspector References ---
    //[Header("Stats")]
    //[SerializeField] private float m_health = 100f;

    [Header("Combat settings")]
    [SerializeField] private GameObject _combatPF;

    // --- Proprierties ---
    public GameObject CombatPF => _combatPF;

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
}
