using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private List<GameObject> m_menus = new();

    [Header("Health bar")]
    [SerializeField] private Transform m_playerUIParent;
    [SerializeField] private Transform m_enemyUIParent;
    [SerializeField] private GameObject m_healthBarPrefab;

    [Header("Inventory")]
    [SerializeField] private Transform m_itemUIParent;
    [SerializeField] private GameObject m_itemPrefab;

    [Header("Abilities")]
    [SerializeField] private Transform m_AbilityUIParent;
    [SerializeField] private GameObject m_AbilityPrefab;

    [Header("Dependency")]
    [SerializeField] private NewBattleManager m_battleManager;
    [SerializeField] private InventoryManager m_inventoryManager;

    // -- Instance ---
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
        if (m_battleManager != null)
            m_battleManager.OnCreateUnit += CreateUnitUI;

        if (m_inventoryManager != null)
            m_inventoryManager.OnRemoveItem += UpdateItemUI;

        

        RebuildUI();
        CreateInvUI();
    }

    private void OnDisable()
    {
        if (m_battleManager != null)
            m_battleManager.OnCreateUnit -= CreateUnitUI;

        if (m_inventoryManager != null)
            m_inventoryManager.OnRemoveItem -= UpdateItemUI;

        foreach (Transform child in m_playerUIParent)
            Destroy(child.gameObject);
        foreach (Transform child in m_enemyUIParent)
            Destroy(child.gameObject);
        foreach (Transform Child in m_itemUIParent)
            Destroy(Child.gameObject);
        foreach (Transform Child in m_AbilityUIParent)
            Destroy(Child.gameObject);
    }

    private void RebuildUI()
    {
        foreach (UnitBase unit in m_battleManager.GetUnits())
            CreateUnitUI(unit);
    }


    public void CreateUnitUI(UnitBase unit)
    {
        Transform parent = (unit.Team == EUnitTeam.Player) ? m_playerUIParent : m_enemyUIParent;
        GameObject ui = Instantiate(m_healthBarPrefab, parent);

        if(ui.TryGetComponent(out UIHealthBar healthBar))
            healthBar.Setup(unit);
    }

    private void CreateInvUI()
    {
        foreach (ItemData itemData in m_inventoryManager.GetItems())
        {
            GameObject itemGO = Instantiate(m_itemPrefab, m_itemUIParent);

            itemGO.name = itemData.Item.name;

            itemGO.TryGetComponent(out Button itemBTN);

            itemBTN.onClick.AddListener(() => m_battleManager.BTNUseItem(itemData));

            itemGO.transform.Find("Item name").TryGetComponent(out TextMeshProUGUI itemNameTMP);
            itemNameTMP.text = itemData.Item.name;

            itemGO.transform.Find("Item qty").TryGetComponent(out TextMeshProUGUI itemQtyTMP);
            itemQtyTMP.text = $"x{itemData.Qty}";
        }
    }

    public void CreateAbilityUI(IReadOnlyList<AbilityData> abilitiesData)
    {
        if (m_AbilityUIParent.childCount != 0)
            foreach (Transform Child in m_AbilityUIParent)
                Destroy(Child.gameObject);
        foreach (AbilityData abilityData in abilitiesData)
        {
            GameObject abilityGO = Instantiate(m_AbilityPrefab, m_AbilityUIParent);

            abilityGO.name = abilityData.Ability.name;

            abilityGO.TryGetComponent(out Button abilityBTN);

            abilityBTN.onClick.AddListener(() => m_battleManager.BTNUseAbility(abilityData));

            abilityGO.transform.Find("Ability name").TryGetComponent(out TextMeshProUGUI abilityNameTMP);
            abilityNameTMP.text = abilityData.Ability.name;

            abilityGO.transform.Find("Ability type").TryGetComponent(out TextMeshProUGUI abilityQtyTMP);
            abilityQtyTMP.text = abilityData.Ability.damageType.ToString();

            abilityGO.transform.Find("Ability charge").TryGetComponent(out TextMeshProUGUI abilityChargeTMP);
            abilityChargeTMP.text = $"Charge {abilityData.ChargeCounter}/{abilityData.Ability.maxCharge}";
        }
    }

    public void UpdateItemUI(ItemData item)
    {
        Transform Child = m_itemUIParent.Find(item.Item.name);
        if (item.Qty <= 0)
        {
            Destroy(Child.gameObject);
        }
        else
        {
            Child.gameObject.transform.Find("Item qty").TryGetComponent(out TextMeshProUGUI itemQtyTMP);
            itemQtyTMP.text = $"x{item.Qty}";
        }
    }

    public void UpdateAbilityUI(AbilityData ability)
    {
        Transform Child = m_itemUIParent.Find(ability.Ability.name);
        Child.gameObject.transform.Find("Ability charge").TryGetComponent(out TextMeshProUGUI abilityChargeTMP);
        abilityChargeTMP.text = $"Charge {ability.ChargeCounter}/{ability.Ability.maxCharge}";
    }

    public void ChangeMenu(int index)
    {
        m_menus.Find(m => m.activeInHierarchy == true).SetActive(false);
        m_menus[index].SetActive(true);
    }
}