using System.Collections.Generic;
using UnityEngine;

public class LoadFileSlotUIBinder : MonoBehaviour
{
    public List<GameObject> slots;

    private void Start()
    {
        LoadFileSlotManager.Instance.BindUISlots(slots);
    }
}
