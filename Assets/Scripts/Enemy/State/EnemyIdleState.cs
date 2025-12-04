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
        _idleTimer += Time.deltaTime;

        if (manager.Movement.IsPlayerVisible())
        {
            manager.SwitchState(manager.chaseState);
            return;
        }

        if (_idleTimer >= Random.Range(_minIdleTime, _maxIdleTime))
        {
            manager.SwitchState(manager.patrolState);
        }
    }

    public override void ExitState(EnemyStateManager enemy) { }

}
