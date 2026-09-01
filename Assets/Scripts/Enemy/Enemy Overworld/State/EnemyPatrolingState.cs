using UnityEngine;

public class EnemyPatrolingState : EnemyBaseState
{
    public override void EnterState(EnemyMovementController manager)
    {
        manager.Movement.SetSpeed(EnemySpeed.Walk);
        manager.Movement.SearchWalkPoint();
    }

    public override void ExitState(EnemyMovementController enemy) { }

    public override void UpdateState(EnemyMovementController manager)
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
