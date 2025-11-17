using System;
using UnityEngine;

// Global Event Bus
public static class GameEvents
{
    // --- Proprierties ---
    public static bool IsInCrafting { get; private set; }
    public static bool IsInInventory { get; private set; }
    public static bool IsInFight { get; private set; }

    // --- Crafting events ---
    public static event Action OnOpenCrafting;
    public static event Action OnCloseCrafting;
    public static event Action<bool> OnCraftingStateChanged;

    // --- Battle events ---
    public static event Action<BattleSettings, GameObject> OnBattleStart;
    public static event Action<BattleResult> OnBattleEnd;

    #region Handle crafting
    public static void SetCraftingState(bool state)
    {
        if (IsInCrafting == state) return;

        IsInCrafting = state;

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
        if (IsInFight) return;

        IsInFight = true;
        OnBattleStart?.Invoke(battleSettings, enemy);
    }

    public static void BattleEnd(BattleResult winner)
    {
        if (!IsInFight) return;

        IsInFight = false;
        OnBattleEnd?.Invoke(winner);
    }
    #endregion
}