using System.Collections;
using UnityEngine;

public class EnemyBusyState : EnemyCombatBaseState
{
    public override void EnterState(EnemyCombatStateManager enemy)
    {
        AttackData ead = enemy.unit.GetAttack();
        enemy.unit.SetAttack(ead);
        enemy.StartCoroutine(ExecuteAttack(enemy));
    }

    public override void ExitState(EnemyCombatStateManager enemy)
    {

    }

    public override void UpdateState(EnemyCombatStateManager enemy)
    {

    }

    private IEnumerator ExecuteAttack(EnemyCombatStateManager enemy)
    {
        yield return BattleManager.Instance.NotifyEnemyAttack();

        enemy.unit.EndTurn();

        BattleManager.Instance.NotifyEnemyFinished(); // importantissimo

        enemy.SwitchState(enemy.WaitingState);
    }
}
