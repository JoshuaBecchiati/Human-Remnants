using System;
using UnityEngine;

[Serializable]
public class DialogueLine 
{
    [SerializeField] private string _speakerName;
    [SerializeField, TextArea(5, 10)] private string _dialogueText;

    public string SpeakerName => _speakerName;
    public string DialogueText => _dialogueText;
}
