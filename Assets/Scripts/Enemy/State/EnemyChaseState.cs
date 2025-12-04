using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        enemy.Movement.SetSpeed(EnemySpeed.Run);
    }

    public override void ExitState(EnemyStateManager enemy) { }

    public override void UpdateState(EnemyStateManager manager)
    {
        if (!manager.Movement.IsPlayerVisible())
        {
            manager.SwitchState(manager.idleState);
            return;
        }

        manager.Movement.ChaseStep();
    }
}
