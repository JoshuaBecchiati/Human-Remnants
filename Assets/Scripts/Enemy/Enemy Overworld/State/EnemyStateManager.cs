using UnityEngine;

// Context dello state machine
public class EnemyStateManager : MonoBehaviour
{
    // --- Inspector ---
    [SerializeField] private float m_postFightCooldown = 2f;

    // --- State ---
    [HideInInspector] public EnemyBaseState CurrentState;
    [HideInInspector] public EnemyIdleState IdleState = new ();
    [HideInInspector] public EnemyPatrolingState PatrolState = new ();
    [HideInInspector] public EnemyChaseState ChaseState = new ();


    // --- Private ---
    private float _cooldownTimer = 0f;

    // --- public ---
    [HideInInspector] public bool IsInPostFightCooldown => _cooldownTimer > 0f;
    public EnemyMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        CurrentState = IdleState;

        CurrentState.EnterState(this);
    }

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        CurrentState.UpdateState(this);
        Movement.AnimateMovement();
    }

    public void SwitchState(EnemyBaseState state)
    {
        CurrentState.ExitState(this);
        CurrentState = state;
        CurrentState.EnterState(this);
    }

    public void StartPostFightCooldown()
    {
        _cooldownTimer = m_postFightCooldown;
    }
}
