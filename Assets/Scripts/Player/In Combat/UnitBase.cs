using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _speed;
    [SerializeField] private float _accumulatedSpeed;
    [SerializeField] private float _speedNextTurn;
    [SerializeField] private float _damage;
    [SerializeField] private bool _isItsTurn;
    [SerializeField] private bool _isDead;
    [SerializeField] private EUnitTeam _team;
    [SerializeField] private Animator _animator;
    [SerializeField] protected UnitBase _target;

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

    public UnitBase Instance { get; private set; }

    public event Action<float, float> OnUnitTookDamage;
    public event Action<float, float> OnHeal;
    public event Action OnEndAttack;
    public event Action<UnitBase> OnDeath;


    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _isDead = false;
    }

    public void StartAttackAnimation(UnitBase target)
    {
        _target = target;
        _animator.SetTrigger("IsAttacking");
    }
    public void EndAttackAnimation()
    {
        OnEndAttack?.Invoke();

    }

    #region Handle health
    public virtual void TakeDamage(float damage)
    {
        _health -= damage;
        OnUnitTookDamage?.Invoke(_health, _maxHealth);
        if (Health <= 0)
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
        Debug.Log($"{_name} has started the turn {_speed}.\n" +
                  $"Current accumulated speed: {_accumulatedSpeed}.");
    }

    public virtual void EndTurn()
    {
        _isItsTurn = false;
        Debug.Log($"{_name} has ended the turn");
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
        Debug.Log($"Reset speed, current accumulated speed: {_accumulatedSpeed}.");
    }
    #endregion

    public void Attack()
    {
        float finalDamage = _damage;
        Debug.Log($"{_name} has attacked {_target._name}. Damage inflicted: {finalDamage}.");
        _target.TakeDamage(finalDamage);

        _target = null;
    }

    public void SetDead() => _isDead = true;
    public void SetAlive() => _isDead = false;
}
