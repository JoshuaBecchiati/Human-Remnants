using System.Collections;

internal interface IAttackExecutor
{
    IEnumerator ExecuteAttack(UnitBase unit);
}
