public abstract class EnemyCombatBaseState
{
    public abstract void ExitState(EnemyCombatStateManager enemy);
    public abstract void EnterState(EnemyCombatStateManager enemy);
    public abstract void UpdateState(EnemyCombatStateManager enemy);
}
