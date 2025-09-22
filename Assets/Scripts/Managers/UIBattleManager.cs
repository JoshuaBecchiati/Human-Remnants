using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleManager : MonoBehaviour
{
    [Header("Menù")]
    [SerializeField] private List<GameObject> m_menus = new();

    [Header("Health bar")]
    [SerializeField] private Transform m_playerUIParent;
    [SerializeField] private Transform m_enemyUIParent;
    [SerializeField] private GameObject m_healthBarPrefab;

    [Header("Inventory")]
    [SerializeField] private Transform m_itemUIParent;
    [SerializeField] private GameObject m_itemPrefab;

    // --- Static ---
    public static UIBattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnCreateUnit += CreateUnitUI;
            RebuildUI();
            CreateInvUI();
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.OnCreateUnit -= CreateUnitUI;

        // se vuoi, pulisci la UI qui
        foreach (Transform child in m_playerUIParent)
            Destroy(child.gameObject);
        foreach (Transform child in m_enemyUIParent)
            Destroy(child.gameObject);
        foreach (Transform Child in m_itemUIParent)
            Destroy(Child.gameObject);
    }

    private void RebuildUI()
    {
        foreach (UnitBase unit in BattleManager.Instance.GetUnits())
            CreateUnitUI(unit);
    }


    public void CreateUnitUI(UnitBase unit)
    {
        Transform parent = (unit.Team == EUnitTeam.player) ? m_playerUIParent : m_enemyUIParent;
        GameObject ui = Instantiate(m_healthBarPrefab, parent);
        if (ui == null) Debug.LogError("HealthBar prefab is NULL!");
        UIHealthBar healthBar = ui.GetComponent<UIHealthBar>();
        if (healthBar == null) Debug.LogError("UIHealthManager mancante nel prefab!");
        healthBar.Setup(unit);
    }

    private void CreateInvUI()
    {
        foreach (ItemData itemData in InventoryManager.Instance.GetItems())
        {
            GameObject itemGO = Instantiate(m_itemPrefab, m_itemUIParent);

            itemGO.name = itemData.Item.name;

            itemGO.TryGetComponent(out Button itemBTN);

            itemBTN.onClick.AddListener(() => BattleManager.Instance.BTNUseItem(itemData));

            itemGO.gameObject.transform.Find("Item name").TryGetComponent(out TextMeshProUGUI itemNameTMP);
            itemNameTMP.text = itemData.Item.name;

            itemGO.gameObject.transform.Find("Item qty").TryGetComponent(out TextMeshProUGUI itemQtyTMP);
            itemQtyTMP.text = $"x{itemData.Qty}";
        }
    }

    //public void CreateAbilityUI(List<AbilityData> abilitiesData)
    //{
    //    foreach (AbilityData abilityData in abilitiesData)
    //    {
    //        GameObject abilityGO = Instantiate(m_itemPrefab, m_itemUIParent);

    //        abilityGO.name = abilityData.ability.name;

    //        abilityGO.TryGetComponent(out Button abilityBTN);

    //        abilityBTN.onClick.AddListener(() => BattleManager.Instance.BTNUseAbility(abilityData));

    //        abilityGO.gameObject.transform.Find("Item name").TryGetComponent(out TextMeshProUGUI abilityNameTMP);
    //        abilityNameTMP.text = abilityData.ability.name;

    //        abilityGO.gameObject.transform.Find("Item type").TryGetComponent(out TextMeshProUGUI abilityQtyTMP);
    //        abilityQtyTMP.text = abilityData.ability.damageType.ToString();

    //        abilityGO.gameObject.transform.Find("Item type").TryGetComponent(out TextMeshProUGUI abilityChargeTMP);
    //        abilityChargeTMP.text = $"Charge{abilityData.chargeCounter}/{abilityData.ability.maxCharge}";
    //    }
    //}

    public void UpdateUI()
    {
        foreach (Transform Child in m_itemUIParent)
            Destroy(Child.gameObject);
        CreateInvUI();
    }

    public void NextMenu(int nextMenu)
    {
        m_menus.Find(m => m.gameObject.activeInHierarchy == true).SetActive(false);
        m_menus[nextMenu].SetActive(true);
    }
}