public abstract class EnemyBaseState
{
    public abstract void ExitState(EnemyMovementController enemy);
    public abstract void EnterState(EnemyMovementController enemy);
    public abstract void UpdateState(EnemyMovementController enemy);
}
