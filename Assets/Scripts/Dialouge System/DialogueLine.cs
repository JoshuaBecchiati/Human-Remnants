using System;
using UnityEngine;

[Serializable]
public class DialogueLine 
{
    [SerializeField] private string _speakerName;
    [SerializeField, TextArea(5, 10)] private string _dialogueText;
    [SerializeField] private DialogueEvent[] _dialogueEvents;

    public string SpeakerName => _speakerName;
    public string DialogueText => _dialogueText;
    public DialogueEvent[] DialogueEvents => _dialogueEvents;

    public void TriggerDialogueEvents()
    {
        if (_dialogueEvents == null)
            return;

        foreach (DialogueEvent dialogueEvent in _dialogueEvents)
        {
            dialogueEvent.TriggerDialogueEvent();
        }
    }
}
