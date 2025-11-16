using System;
using UnityEngine;

// Global Event Bus
public static class GameEvents
{
   // --- Private ---
    private static bool _isInFight = false;

    // --- Proprierties ---
    public static bool IsInCrafting { get; private set; }
    public static bool IsInInventory { get; private set; }

    // --- Crafting events ---
    public static event Action OnOpenCrafting;
    public static event Action OnCloseCrafting;
    public static event Action<bool> OnCraftingStateChanged;


    // --- Battle events ---
    public static event Action<BattleSettings, GameObject> OnBattleStart;
    public static event Action OnBattleEnd;

    #region Handle crafting
    public static void SetCraftingState(bool state)
    {
        if (IsInCrafting == state) return;

        IsInCrafting = state;

        if (IsInCrafting)
            Debug.Log("[GameEvents] Crafting opened");
        else
            Debug.Log("[GameEvents] Crafting closed");

        OnCraftingStateChanged?.Invoke(state);
    }

    public static void SetCraftingOpened(bool state)
    {
        if (IsInInventory == state) return;

        IsInInventory = state;

        if (IsInInventory)
            OnOpenCrafting?.Invoke();
        else
            OnCloseCrafting?.Invoke();
    }
    #endregion

    #region Handle Battle
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
    #endregion
}