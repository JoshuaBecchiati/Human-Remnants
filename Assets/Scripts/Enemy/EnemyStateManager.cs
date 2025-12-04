using UnityEngine;

// Context dello state machine
public class EnemyStateManager : MonoBehaviour
{
    [HideInInspector] public EnemyBaseState currentState;
    [HideInInspector] public EnemyIdleState idleState = new ();
    [HideInInspector] public EnemyPatrolingState patrolState = new ();
    [HideInInspector] public EnemyChaseState chaseState = new ();

    public EnemyMovement Movement { get; private set; }

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        currentState = idleState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    private void Update()
    {
        currentState.UpdateState(this);
        Movement.AnimateMovement();
    }

    public void SwitchState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
