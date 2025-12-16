using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewDescBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        IReadOnlyList<ItemData> items = InventoryManager.Instance.GetItems();
        string itemName = transform.Find("Item name").GetComponent<TextMeshProUGUI>().text;

        foreach (ItemData item in items)
        {
            if (item.Item.name == itemName)
            {
                UIInventoryManager.Instance.ShowDescription(item.Item);
                break;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIInventoryManager.Instance.HideDescription();
    }
}
