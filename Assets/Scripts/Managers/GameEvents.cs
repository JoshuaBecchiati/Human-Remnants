using System;
using UnityEngine;

// Global Event Bus
public static class GameEvents
{
    private static bool _isInFight = false;

    public static event Action<BattleSettings> OnBattleStart;
    public static event Action OnBattleEnd;

    public static void BattleStart(BattleSettings battleSettings)
    {
        if (!_isInFight)
        {
            _isInFight = true;
            OnBattleStart?.Invoke(battleSettings);
        }
    }

    public static void BattleEnd(EUnitTeam winnerTeam)
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
