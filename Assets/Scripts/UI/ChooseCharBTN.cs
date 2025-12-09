using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCharBTN : CustomAnimatedBTN
{
    [SerializeField] private GameObject _charInfo;

    private void Awake()
    {
        _charInfo.SetActive(false);
    }

    private void OnDisable()
    {
        _charInfo.SetActive(false);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        _charInfo.SetActive(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _charInfo.SetActive(false);
    }
}
