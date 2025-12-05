using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Animator m_animator;

    [Header("Ranges")]
    [SerializeField] private float sight = 6f;
    [SerializeField] private float walkRange = 3f;

    [Header("Layers")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private LayerMask whatIsGround;

    private Transform _player;
    private Vector3 _walkPoint;
    private Vector3 _centerPoint;
    private bool _isWalkPointSet;

    private Collider[] _playerHits = new Collider[5];

    #region Unity Methods
    private void OnValidate()
    {
        if (!m_agent) m_agent = GetComponent<NavMeshAgent>();
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _centerPoint = transform.position;
        m_agent.speed = (float)EnemySpeed.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, walkRange);
    }
    #endregion

    #region Speed Control
    public void SetSpeed(EnemySpeed speed)
    {
        m_agent.speed = (float)speed;
    }
    #endregion

    #region Player Detection
    public bool IsPlayerVisible()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, sight, _playerHits, whatIsPlayer);
        if (count > 0)
        {
            _player = _playerHits[0].transform;
            return true;
        }
        _player = null;
        return false;
    }

    public float DistanceToPlayer()
    {
        return _player ? Vector3.Distance(transform.position, _player.position) : Mathf.Infinity;
    }
    #endregion

    #region Patrol
    public void SearchWalkPoint()
    {
        Vector3 randomPoint = _centerPoint + new Vector3(Random.Range(-walkRange, walkRange), 0, Random.Range(-walkRange, walkRange));
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            _walkPoint = hit.position;
            _isWalkPointSet = true;
        }
    }

    public bool ReachedWalkPoint()
    {
        if (!_isWalkPointSet) return false;

        if ((transform.position - _walkPoint).sqrMagnitude < 1f)
        {
            _isWalkPointSet = false;
            return true;
        }
        return false;
    }

    public void PatrolStep()
    {
        if (_isWalkPointSet)
            m_agent.SetDestination(_walkPoint);
    }
    #endregion

    #region Chase
    public void ChaseStep()
    {
        if (!_player) return;
        m_agent.SetDestination(_player.position);
    }
    #endregion

    #region Animation
    public void AnimateMovement()
    {
        m_animator.SetFloat("Y", m_agent.velocity.magnitude);
    }
    #endregion
}

