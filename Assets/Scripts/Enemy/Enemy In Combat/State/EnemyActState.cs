public class EnemyActState : EnemyCombatBaseState
{
    public override void EnterState(EnemyCombatStateManager enemy)
    {
        if (enemy.unit.IsDead)
        {
            enemy.SwitchState(enemy.DyingState);
            return;
        }

        // 1. Sceglie il bersaglio
        UnitBase target = BattleManager.Instance.SelectEnemyTarget();
        enemy.unit.SetTarget(target);

        // 2. Passa a busy (dove farà l'attacco)
        enemy.SwitchState(enemy.BusyState);
    }

    public override void ExitState(EnemyCombatStateManager enemy)
    {

    }

    public override void UpdateState(EnemyCombatStateManager enemy)
    {

    }
}
