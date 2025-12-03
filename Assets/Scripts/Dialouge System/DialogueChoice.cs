using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class DialogueChoice
{
    [SerializeField] private string _choiceText;
    [SerializeField] private NovelDialogue _nextDialogue;
    public UnityEvent OnCoichePicked;

    public string ChoiceText => _choiceText;
    public NovelDialogue NextDialogue => _nextDialogue;
}

