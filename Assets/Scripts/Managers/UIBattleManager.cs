using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleManager : MonoBehaviour
{
    [SerializeField] private Transform playerUIParent;
    [SerializeField] private Transform enemyUIParent;
    [SerializeField] private GameObject healthBarPrefab;

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnCreateUnit += CreateUnitUI;
            RebuildUI(); // nel caso alcune unità siano già state create
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnCreateUnit -= CreateUnitUI;

        // se vuoi, pulisci la UI qui
        foreach (Transform child in playerUIParent)
            Destroy(child.gameObject);
        foreach (Transform child in enemyUIParent)
            Destroy(child.gameObject);
    }

    private void RebuildUI()
    {
        foreach (var unit in BattleManager.Instance.GetUnits())
            CreateUnitUI(unit);
    }


    public void CreateUnitUI(UnitBase unit)
    {
        Transform parent = (unit.Team == EUnitTeam.player) ? playerUIParent : enemyUIParent;
        var ui = Instantiate(healthBarPrefab, parent);
        if (ui == null) Debug.LogError("HealthBar prefab is NULL!");
        var healthBar = ui.GetComponent<UIHealthBar>();
        if (healthBar == null) Debug.LogError("UIHealthManager mancante nel prefab!");
        healthBar.Setup(unit);
    }
}