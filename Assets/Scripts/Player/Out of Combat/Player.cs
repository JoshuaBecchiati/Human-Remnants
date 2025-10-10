using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharController, IDamageable
{
    // --- Inspector References ---
    [Header("Stats")]
    [SerializeField] private float m_health = 100;
    [SerializeField] private float m_invicibleTime = 5f;

    [Header("Combat settings")]
    [SerializeField] private GameObject _combatPF;

    // --- Private ---
    private bool _isInvincible;

    // --- Proprierties ---
    public GameObject CombatPF => _combatPF;

    // --- Events ---
    public event Action OnDeath;
    public event Action<float> OnChangeHealth;

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    /*
     * 
     * [Damage Handling]
     * 
     */

    public void TakeDamage(float amount)
    {
        if (!_isInvincible)
        {
            m_health -= amount;
            StartCoroutine(Invincibility());
            OnChangeHealth?.Invoke(m_health);
        }
        if (m_health <= 0)
        {
            // Avviso gli altri script in ascolto della morte del player
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    private IEnumerator Invincibility()
    {
        _isInvincible = true;

        // Disattiva il collider per alcuni secondi cosicché il giocatore può eventualmente scappare da una fight

        yield return new WaitForSeconds(m_invicibleTime);

        _isInvincible = false;
    }

}
