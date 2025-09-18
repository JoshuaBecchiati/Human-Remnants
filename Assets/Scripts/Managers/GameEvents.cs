using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<GameObject, GameObject> OnBattleEnter;
    public static void RaiseBattleEnter(GameObject battle, GameObject player) => OnBattleEnter?.Invoke(battle, player);
}
