using System;
using UnityEngine;

// Global Event Bus
public static class GameEvents
{
    private static bool _isInFight = false;

    public static event Action OnBattleStart;
    public static event Action OnBattleEnd;

    public static void BattleStart()
    {
        if (!_isInFight)
        {
            _isInFight = true;
            OnBattleStart?.Invoke();
        }
    }

    public static void BattleEnd()
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
