using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class DialogueChoiceButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_Text _buttonText;

    private int _choiceIndex;

    public event Action<int> OnChoiceSelected;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnChoiceSelected?.Invoke(_choiceIndex);
    }
    
    public void SetChoiceText(string text, int index)
    {
        _choiceIndex = index;
        _buttonText.text = text;
    }
}