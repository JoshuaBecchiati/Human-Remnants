using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    private float _idleTimer;
    private float _minIdleTime = 2.5f;
    private float _maxIdleTime = 5f;

    public override void EnterState(EnemyStateManager manager)
    {
        _idleTimer = 0f;
        manager.Movement.SetSpeed(EnemySpeed.Idle);
    }

    public override void UpdateState(EnemyStateManager manager)
    {
        if (manager.IsInPostFightCooldown)
            return;

        if (manager.Movement.IsPlayerVisible())
        {
            manager.SwitchState(manager.ChaseState);
            return;
        }

        _idleTimer += Time.deltaTime;
        if (_idleTimer >= Random.Range(_minIdleTime, _maxIdleTime))
        {
            manager.SwitchState(manager.PatrolState);
        }
    }

    public override void ExitState(EnemyStateManager enemy) { }

}
