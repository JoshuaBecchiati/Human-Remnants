using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    // --- Inspector ---
    [Header("Stats")]
    [SerializeField] protected string m_name;
    [SerializeField] private float m_health;
    [SerializeField] private float m_maxHealth;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_accumulatedSpeed;
    [SerializeField] private float m_speedNextTurn;
    [SerializeField] private float m_damage;
    [SerializeField] private UnitTeam m_team;

    [Header("Animations")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private TimelineDatabase m_timeLineDB;
    [SerializeField] private List<AttackData> m_attackDatas;

    // --- Private ---
    [SerializeField] private UnitBase _target;
    private bool _isItsTurn;
    [SerializeField] private bool _isDead;

    // --- Prorprierties ---
    public string Name => m_name;
    public float Health => m_health;
    public float MaxHealth => m_maxHealth;
    public float Speed => m_speed;
    public float AccumulatedSpeed => m_accumulatedSpeed;
    public float SpeedNextTurn => m_speedNextTurn;
    public float Damage => m_damage;
    public bool IsItsTurn => _isItsTurn;
    public bool IsDead => _isDead;
    public UnitTeam Team => m_team;
    public Animator Animator => m_animator;
    public TimelineDatabase TimeLineDB => m_timeLineDB;
    public UnitBase Target => _target;
    public List<AttackData> AttackDatas => m_attackDatas;


    // --- Events ---
    public event Action<float, float> OnUnitTookDamage;
    public event Action<float, float> OnHeal;
    public event Action OnEndAttack;
    public event Action<UnitBase> OnDeath;

    protected virtual void Awake()
    {
        _isDead = false;
    }

    private void OnValidate()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    public void StartAttackAnimation(UnitBase target)
    {
        _target = target;
        m_animator.SetTrigger("IsAttacking");
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
        m_health -= damage;
        m_animator.SetTrigger("IsHitted");

        if (m_health <= 0)
        {
            m_animator.SetTrigger("IsDying");
            OnDeath?.Invoke(this);
        }

        OnUnitTookDamage?.Invoke(m_health, m_maxHealth);
    }

    public virtual void Heal(float heal)
    {
        if (m_health + heal > m_maxHealth)
            m_health = m_maxHealth;
        else if (m_health == m_maxHealth)
            Debug.Log("You're full health");
        else
            m_health += heal;
        OnHeal?.Invoke(m_health, m_maxHealth);
    }

    public virtual void SetHealth(float health)
    {
        m_health = health;
    }
    #endregion

    #region Handle start and end turn
    public virtual void StartTurn()
{
        _isItsTurn = true;
        m_accumulatedSpeed += Speed;
    }

    public virtual void EndTurn()
    {
        _isItsTurn = false;
    }
    #endregion

    #region Handle speed
    public virtual void SetSpeed(int speed)
    {
        m_speed = speed;
    }

    public virtual void ResetAccumulatedSpeed()
    {
        m_accumulatedSpeed = 0;
    }
    #endregion

    public void Attack()
    {
        float finalDamage = m_damage;

        _target.TakeDamage(finalDamage);
    }
    public void SetTarget(UnitBase target) => _target = target;
    public void SetDead() => _isDead = true;
    public void SetAlive() => _isDead = false;
}
