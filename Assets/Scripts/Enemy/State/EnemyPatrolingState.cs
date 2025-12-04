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
            manager.SwitchState(manager.chaseState);
            return;
        }

        if (manager.Movement.ReachedWalkPoint())
        {
            manager.SwitchState(manager.idleState);
            return;
        }

        manager.Movement.PatrolStep();
    }
}
