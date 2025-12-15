using System;
using UnityEngine;

[Serializable]
public class DialogueChoice
{
    [SerializeField] private string _choiceText;
    [SerializeField] private Dialogue _nextDialogue;

    public string ChoiceText => _choiceText;
    public Dialogue NextDialogue => _nextDialogue;
}

