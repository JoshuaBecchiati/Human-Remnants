using UnityEngine;

[CreateAssetMenu(fileName = "New Novel Dialogue", menuName = "Visual Novel Engine/Novel Dialogue")]
public class NovelDialogue : ScriptableObject
{
    [SerializeField] private DialogueLine[] _dialogueLines;
    [SerializeField] private DialogueChoice[] _dialogueChoices;

    public DialogueChoice[] DialogueChoices => _dialogueChoices;
    public bool IsEndDialogue => _dialogueChoices == null || _dialogueChoices.Length == 0;
    public int DialogueLineCount => _dialogueLines.Length;

    public DialogueLine GetDialogueLine(int index)
    {
        if (index < 0 || index >= _dialogueLines.Length)
        {
            Debug.LogError("Dialogue line index out of range.");
            return null;
        }
        return _dialogueLines[index];
    }


}
