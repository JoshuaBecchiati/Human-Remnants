using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NovelGUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private GameObject m_dialogueBox;
    [SerializeField] private DialogueChoiceButton _dialogueChoiceButtonPrefab;
    [SerializeField] private Transform _dialogueChoicesContainer;
    [SerializeField] private float m_charsPerSecond = 30f;

    private NovelDialogue _currentDialogue;
    private int _currentDialogueLineIndex = 0;
    private Coroutine _dialogueEffect;

    public UnityEvent OnDialogueAdvanced;
    public UnityEvent OnSceneEnded;

    public static NovelGUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_dialogueBox.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Handle pointer down events (e.g., advancing dialogue)
        UpdateToNextDialogueLine();
        OnDialogueAdvanced?.Invoke();
    }

    public void StartDialogue(NovelDialogue dialogue)
    {
        m_dialogueBox.SetActive(true);
        SetCurrentDialogue(dialogue);
    }

    public void EndDialogue()
    {
        GameEvents.SetDialogueState(false);
        m_dialogueBox.SetActive(false);
    }

    public void SetCurrentDialogue(NovelDialogue dialogue)
    {
        GameEvents.SetDialogueState(true);
        _currentDialogue = dialogue;
        _currentDialogueLineIndex = -1;
        UpdateToNextDialogueLine();
    }

    private void UpdateToNextDialogueLine()
    {
        if (_currentDialogue == null)
            return;

        if(_currentDialogueLineIndex + 1 >= _currentDialogue.DialogueLineCount)
        {
            DisplayChoices(_currentDialogue);
            return;
        }

        _currentDialogueLineIndex++;
        UpdateDialogueUI(_currentDialogue.GetDialogueLine(_currentDialogueLineIndex));
    }

    public void UpdateDialogueUI(DialogueLine dialogueLine)
    {
        _speakerNameText.text = dialogueLine.SpeakerName;

        if (_dialogueEffect != null)
            StopCoroutine(_dialogueEffect);

        _dialogueEffect = StartCoroutine(TypeWriterEffect(dialogueLine));
    }

    private IEnumerator TypeWriterEffect(DialogueLine dialogueLine)
    {
        string text = dialogueLine.DialogueText;
        _dialogueText.text = "";

        float t = 0f;
        int lastChar = 0;

        while (lastChar < text.Length)
        {
            t += Time.deltaTime;

            // quanti caratteri dovremmo aver mostrato fino a questo momento?
            int charCount = Mathf.FloorToInt(t * m_charsPerSecond);

            if (charCount != lastChar)
            {
                lastChar = Mathf.Clamp(charCount, 0, text.Length);
                _dialogueText.text = text.Substring(0, lastChar);
            }

            yield return null;
        }
    }

    private void DisplayChoices(NovelDialogue dialogue)
    {
        if(_currentDialogue.IsEndDialogue)
        {
            OnSceneEnded?.Invoke();
            return;
        }

        // Clear existing choice buttons
        foreach (Transform child in _dialogueChoicesContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new choice buttons
        DialogueChoice[] choices = dialogue.DialogueChoices;
        for (int i = 0; i < choices.Length; i++)
        {
            DialogueChoiceButton choiceButton = Instantiate(_dialogueChoiceButtonPrefab, _dialogueChoicesContainer);
            choiceButton.SetChoiceText(choices[i].ChoiceText, i);
            choiceButton.OnChoiceSelected += OnChoiceSelected;
        }
    }

    private void OnChoiceSelected(int choiceIndex)
    { 
        foreach (Transform child in _dialogueChoicesContainer)
        {
            Destroy(child.gameObject);
        }

        DialogueChoice choice = _currentDialogue.DialogueChoices[choiceIndex];
        if (choice.NextDialogue != null)
        {
            SetCurrentDialogue(choice.NextDialogue);
        }
    }
}