using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharCtrl, IDamageable
{
    // --- Inspector References ---
    [Header("Stats")]
    [SerializeField] private float m_health = 100;
    [SerializeField] private float m_stamina = 100;
    [SerializeField] private float m_maxStamina = 100;
    [SerializeField] private float m_staminaRegenRate = 10f;
    [SerializeField] private float m_staminaDrainRun = 5f;
    [SerializeField] private float m_invicibleTime = 5f;

    [Header("Combat settings")]
    [SerializeField] private GameObject _combatPF;

    // --- Private ---
    private bool _isInvincible;
    private bool _isRunning;

    // --- Proprierties ---
    public GameObject CombatPF => _combatPF;

    // --- Events ---
    public event Action OnDeath;
    public event Action<float> OnChangeHealth;
    public event Action<float> OnChangeStamina;
    public event Action<float> OnRefillStamina;

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleStamina();
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

    /*
     * 
     * [Run & Stamina Handling]
     * 
     */

    protected override void OnSprintStarted(InputAction.CallbackContext context)
    {
        if (m_stamina > 0)
        {
            _isRunning = true;
            base.OnSprintStarted(context);
            StartCoroutine(DrainStaminaWhileRunning());
        }
    }

    protected override void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _isRunning = false;
        base.OnSprintCanceled(context);
    }

    private void HandleStamina()
    {
        // Rigenerazione se non sto correndo né sparando
        if (m_stamina < m_maxStamina && !_isRunning)
        {
            m_stamina += m_staminaRegenRate * Time.deltaTime;
            m_stamina = Mathf.Min(m_stamina, m_maxStamina);
            OnRefillStamina?.Invoke(m_stamina);
        }
    }

    private IEnumerator DrainStaminaWhileRunning()
    {
        while (_speedMagnitude == m_runSpeed && m_stamina > 0)
        {
            m_stamina -= m_staminaDrainRun * Time.deltaTime;
            OnChangeStamina?.Invoke(m_stamina);

            if (m_stamina <= 0)
            {
                m_stamina = 0;
                _speedMagnitude = m_walkSpeed;
            }
            yield return null;
        }
    }
}
