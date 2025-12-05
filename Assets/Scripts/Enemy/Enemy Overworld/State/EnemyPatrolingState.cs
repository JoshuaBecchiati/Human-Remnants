using UnityEngine;

public class EnemyPatrolingState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager manager)
    {
        manager.Movement.SetSpeed(EnemySpeed.Walk);
        manager.Movement.SearchWalkPoint();
    }

    public override void ExitState(EnemyStateManager enemy) { }

    public override void UpdateState(EnemyStateManager manager)
    {
        if (manager.Movement.IsPlayerVisible())
        {
            manager.SwitchState(manager.ChaseState);
            return;
        }

        if (manager.Movement.ReachedWalkPoint() || manager.IsInPostFightCooldown)
        {
            manager.SwitchState(manager.IdleState);
            return;
        }

        manager.Movement.PatrolStep();
    }
}
