using UnityEngine;

public class EnemyCombatStateManager : MonoBehaviour
{
    // --- State ---
    [HideInInspector] public EnemyCombatBaseState CurrentState;
    [HideInInspector] public EnemyWaitingState WaitingState = new ();
    [HideInInspector] public EnemyBusyState BusyState = new ();
    [HideInInspector] public EnemyActState ActState = new ();
    [HideInInspector] public EnemyDyingState DyingState = new ();

    // --- Public ---
    public UnitBase unit;

    private void OnValidate()
    {
        unit = GetComponent<UnitBase>();
    }

    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<UnitBase>();
        CurrentState = WaitingState;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.UpdateState(this);
    }

    public void SwitchState(EnemyCombatBaseState state)
    {
        CurrentState.ExitState(this);
        CurrentState = state;
        CurrentState.EnterState(this);
    }
}
