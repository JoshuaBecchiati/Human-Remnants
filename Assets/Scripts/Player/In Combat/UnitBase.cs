using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public abstract class UnitBase : MonoBehaviour
{
    // --- Inspector ---
    [Header("Stats")]
    [SerializeField] protected string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _speed;
    [SerializeField] private float _accumulatedSpeed;
    [SerializeField] private float _speedNextTurn;
    [SerializeField] private float _damage;
    [SerializeField] private EUnitTeam _team;

    [Header("Animations")]
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayableDirector _attackCinematic;
    [SerializeField] private TimelineAsset _baseAttack;
    [SerializeField] private SignalReceiver _sr;

    // --- Private ---
    [SerializeField] private UnitBase _target;
    private bool _isItsTurn;
    [SerializeField] private bool _isDead;

    // --- Prorprierties ---
    public string Name => _name;
    public float Health => _health;
    public float MaxHealth => _maxHealth;
    public float Speed => _speed;
    public float AccumulatedSpeed => _accumulatedSpeed;
    public float SpeedNextTurn => _speedNextTurn;
    public float Damage => _damage;
    public bool IsItsTurn => _isItsTurn;
    public bool IsDead => _isDead;
    public EUnitTeam Team => _team;
    public PlayableDirector AttackCinematic => _attackCinematic;
    public SignalReceiver SignalReceiver => _sr;
    public TimelineAsset BaseAttack => _baseAttack;
    public UnitBase Target => _target;


    // --- Events ---
    public event Action<float, float> OnUnitTookDamage;
    public event Action<float, float> OnHeal;
    public event Action OnEndAttack;
    public event Action<UnitBase> OnDeath;


    protected virtual void Awake()
    {
        _isDead = false;
    }

    public void StartAttackAnimation(UnitBase target)
    {
        _target = target;
        _animator.SetTrigger("IsAttacking");
    }

    public void StartAttackCinematic(UnitBase target)
    {
        _target = target;
    }

    public void EndAttackAnimation()
    {
        OnEndAttack?.Invoke();
    }

    #region Handle health
    public virtual void TakeDamage(float damage)
    {
        _health -= damage;
        _animator.SetTrigger("IsHitted");

        if (_health <= 0)
        {
            StartCoroutine(DieSequence());
        }

        OnUnitTookDamage?.Invoke(_health, _maxHealth);
    }

    private IEnumerator DieSequence()
    {
        _animator.SetTrigger("IsDying");

        // Attendi che l'animazione finisca o il segnale arrivi
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        HandleDeath();
    }

    public virtual void HandleDeath()
    {
        OnDeath?.Invoke(this);
    }

    public virtual void Heal(float heal)
    {
        if (_health + heal > _maxHealth)
            _health = _maxHealth;
        else if (_health == _maxHealth)
            Debug.Log("You're full health");
        else
            _health += heal;
        OnHeal?.Invoke(_health, _maxHealth);
    }
    #endregion

    #region Handle start and end turn
    public virtual void StartTurn()
{
        _isItsTurn = true;
        _accumulatedSpeed += Speed;
    }

    public virtual void EndTurn()
    {
        _isItsTurn = false;
    }
    #endregion

    #region Handle speed
    public virtual void SetSpeed(int speed)
    {
        _speed = speed;
    }

    public virtual void ResetAccumulatedSpeed()
    {
        _accumulatedSpeed = 0;
    }
    #endregion

    public void Attack()
    {
        float finalDamage = _damage;

        _target.TakeDamage(finalDamage);
    }
    public void SetTarget(UnitBase target) => _target = target;
    public void SetDead() => _isDead = true;
    public void SetAlive() => _isDead = false;
}
