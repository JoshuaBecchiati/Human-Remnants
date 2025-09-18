using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] protected float _health = 100f;
    [SerializeField] protected float _damage = 10f;
    [SerializeField] protected int _score = 10;

    [Header("Components")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Image _healthBar;

    [Header("Movement")]
    [SerializeField] protected bool _isPlayerInSight;
    [SerializeField] private bool _isWalkPointSet;
    [SerializeField] protected bool _isInAttackRange;
    [SerializeField] private bool _isRepositioning;

    [SerializeField] private float _walkRange = 5f;
    [SerializeField] private float _sightRange = 5f;
    [SerializeField] private float _distanceFromPlayer = 2.5f;
    [SerializeField] private float _stopDistance = 1.5f;
    [SerializeField] protected float _attackRange = 4f;
    [SerializeField] private float _repositionCooldown = 1f;

    [SerializeField] protected Vector3 _newPoint;
    [SerializeField] private LayerMask _whatIsPlayer;
    [SerializeField] private LayerMask _whatIsGround;

    protected float _fireRateCoolDown;

    public Transform Player => _player;
    public NavMeshAgent Agent => _agent;
    public int Score => _score; 

    public event Action<IEnemy> OnDeath;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player").transform;

    }

    /*
     * 
     * [Movement Handler]
     * 
     */

    protected virtual void Movement()
    {
        _isPlayerInSight = Physics.CheckSphere(transform.position, _sightRange, _whatIsPlayer);


        if (_isPlayerInSight)
        {
            if (IsAttacking())
            {
                if (IsNotAttackOccluded())
                {
                    // Player visibile → resta alla distanza giusta
                    float distance = Vector3.Distance(transform.position, _player.position);
                    if (distance < _stopDistance) RunAway();
                    else if (distance > _distanceFromPlayer) ChasePlayer();
                    else _agent.ResetPath();
                }
                else Reposition(); // Player attaccabile ma occluso → cerca di spostarsi
            }
            else ChasePlayer(); // Player visibile ma fuori range → inseguire
        }
        else Patroling(); // Player non visibile → pattuglia
    }

    private void ChasePlayer()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        Vector3 targetPosition = _player.position - direction * _distanceFromPlayer;
        _agent.destination = targetPosition;

        _agent.updateRotation = true;
    }

    private void Patroling()
    {
        if (!_isWalkPointSet) SearchWalkPoint();

        if (_isWalkPointSet) _agent.SetDestination(_newPoint);

        Vector3 toWalkPoint = transform.position - _newPoint;

        if(toWalkPoint.magnitude < 1f)
            _isWalkPointSet = false;

    }

    private void SearchWalkPoint()
    {
        float randZ = UnityEngine.Random.Range(-_walkRange, _walkRange);
        float randX = UnityEngine.Random.Range(-_walkRange, _walkRange);
        _newPoint = new Vector3(_newPoint.x + randX, _newPoint.y, _newPoint.z + randZ);

        if (Physics.Raycast(_newPoint, -transform.up, _whatIsGround))
            _isWalkPointSet = true;
    }

    private void RunAway()
    {
        Vector3 direction = (transform.position - _player.position).normalized;

        float fleeDistance = _stopDistance * 2f;
        Vector3 targetPosition = transform.position + direction * fleeDistance;

        _agent.SetDestination(targetPosition);

        transform.LookAt(_player);
    }

    /*
     * 
     * [Repositioning]
     * 
     */

    private void Reposition()
    {
        if (_isRepositioning) return;
        _isRepositioning = true;

        Vector3 directionToPlayer = (_player.position - transform.position).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, directionToPlayer).normalized;

        Vector3 chosenDir = (UnityEngine.Random.value > 0.5f) ? right : -right;
        Vector3 targetPos = transform.position + chosenDir * 2f;

        _agent.SetDestination(targetPos);

        Invoke(nameof(ResetReposition), _repositionCooldown);
    }

    private void ResetReposition()
    {
        _isRepositioning = false;
    }

    /*
     * 
     * [Attack]
     * 
     */

    protected bool IsAttacking()
    {
        return _isInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _whatIsPlayer);
    }

    protected bool IsNotAttackOccluded()
    {
        Vector3 direction = (_player.position - transform.position).normalized;
        if (Physics.SphereCast(transform.position + Vector3.up, 0.25f, direction, out RaycastHit hit, _attackRange))
        {
            if (hit.transform.CompareTag("Player"))
                return true; // Player visibile
            else
                return false; // Ostacolo davanti
        }
        return false; // Nessun ostacolo né player
    }

    /*
     * 
     * [Health Handling]
     * 
     */

    public void TakeDamage(float amount)
    {
        _health -= amount;
        UpdateHealthUI();
        if (_health <= 0)
        {
            OnInvokeOnDeath();
            Destroy(gameObject);
        }
    }
    private void UpdateHealthUI()
    {
            _healthBar.fillAmount = _health / _maxHealth;
    }


    /*
     * 
     * [Events Handling]
     * 
     */

    protected virtual void OnInvokeOnDeath()
    {
        OnDeath?.Invoke(this);
    }
}
