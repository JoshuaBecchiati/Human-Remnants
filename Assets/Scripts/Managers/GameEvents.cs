using System;
using UnityEngine;

// Global Event Bus
public static class GameEvents
{
    private static bool _isInFight = false;

    public static event Action<BattleSettings, GameObject> OnBattleStart;
    public static event Action OnBattleEnd;

    public static void BattleStart(BattleSettings battleSettings, GameObject enemy)
    {
        if (!_isInFight)
        {
            _isInFight = true;
            OnBattleStart?.Invoke(battleSettings, enemy);
        }
    }

    public static void BattleEnd(BattleResult winner)
    {
        if (_isInFight)
        {
            _isInFight = false;
            OnBattleEnd?.Invoke();
        }
    }


    public static event Action<GameObject, GameObject> OnBattleEnter;
    public static void RaiseBattleEnter(GameObject battle, GameObject player) => OnBattleEnter?.Invoke(battle, player);
}
