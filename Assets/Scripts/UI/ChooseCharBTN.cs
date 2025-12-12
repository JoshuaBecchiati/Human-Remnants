using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChooseCharBTN : CustomAnimatedBTN
{
    [SerializeField] private GameObject _charInfo;
    [SerializeField] private List<ChooseCharBTN> _charList;

    public GameObject CharInfo => _charInfo;

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

        if (_isOpen)
        {
            ChooseCharBTN cl = _charList.Find(o => o.CharInfo.activeSelf && o.gameObject != this);
            if (cl != null)
            {
                cl.SetCharInfoState(false);
            }
            _charInfo.SetActive(true);
        }
        else
            _charInfo.SetActive(false);
    }

    public void SetCharInfoState(bool state)
    {
        _charInfo.SetActive(state);
    }
}
