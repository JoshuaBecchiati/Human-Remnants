public class EnemyChaseState : EnemyBaseState
{
    public override void EnterState(EnemyMovementController enemy)
    {
        enemy.Movement.SetSpeed(EnemySpeed.Run);
    }

    public override void ExitState(EnemyMovementController enemy) { }

    public override void UpdateState(EnemyMovementController manager)
    {
        if (manager.IsInPostFightCooldown)
        {
            manager.SwitchState(manager.IdleState);
            return;
        }

        if (!manager.Movement.IsPlayerVisible())
        {
            manager.SwitchState(manager.IdleState);
            return;
        }

        manager.Movement.ChaseStep();
    }
}
