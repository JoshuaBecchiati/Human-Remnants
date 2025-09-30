using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField] private List<ItemData> m_craftingList = new ();
    [SerializeField] private GameObject[] m_itemSlot = new GameObject[2];
    [SerializeField] private GameObject m_result;

    [Header("Dependencies")]
    [SerializeField] private InventoryManager m_inventory;
    [SerializeField] private UIBattleManager m_uiBattle;

    public void GetItemInInventory()
    {

    }
}
