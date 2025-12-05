public class EnemyChaseState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        enemy.Movement.SetSpeed(EnemySpeed.Run);
    }

    public override void ExitState(EnemyStateManager enemy) { }

    public override void UpdateState(EnemyStateManager manager)
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
