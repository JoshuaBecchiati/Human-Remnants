using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharCtrl, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float _health = 100;
    [SerializeField] private float _stamina = 100;
    [SerializeField] private float _maxStamina = 100;
    [SerializeField] private float _staminaRegenRate = 10f;
    [SerializeField] private float _staminaDrainRun = 5f;
    [SerializeField] private float _invicibleTime = 5f;

    [SerializeField] private bool _isInvincible;
    [SerializeField] private bool _isRunning;

    public event Action OnDeath;
    public event Action<float> OnChangeHealth;
    public event Action<float> OnChangeStamina;
    public event Action<float> OnRefillStamina;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

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
            _health -= amount;
            StartCoroutine(Invincibility());
            OnChangeHealth?.Invoke(_health);
        }
        if (_health <= 0)
        {
            // Avviso gli altri script in ascolto della morte del player
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    private IEnumerator Invincibility()
    {
        _isInvincible = true;

        Renderer baseColor = GetComponent<Renderer>();
        Color savedColor = baseColor.material.color;
        baseColor.material.color = Color.yellow;

        yield return new WaitForSeconds(_invicibleTime);

        _isInvincible = false;

        baseColor.material.color = savedColor;
    }

    /*
     * 
     * [Run & Stamina Handling]
     * 
     */

    protected override void OnSprintStarted(InputAction.CallbackContext context)
    {
        // Correre consuma stamina
        if (_stamina > 0)
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
        if (_stamina < _maxStamina && !_isRunning)
        {
            _stamina += _staminaRegenRate * Time.deltaTime;
            _stamina = Mathf.Min(_stamina, _maxStamina);
            OnRefillStamina?.Invoke(_stamina);
        }
    }

    private IEnumerator DrainStaminaWhileRunning()
    {
        while (_speedMagnitude == _runSpeed && _stamina > 0)
        {
            _stamina -= _staminaDrainRun * Time.deltaTime;
            OnChangeStamina?.Invoke(_stamina);

            if (_stamina <= 0)
            {
                // stamina finita → ritorno a camminare
                _stamina = 0;
                _speedMagnitude = _walkSpeed;
            }
            yield return null;
        }
    }
}
