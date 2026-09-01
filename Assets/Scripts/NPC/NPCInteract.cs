using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteract : SaveableObject
{
    [Header("Settings")]
    [SerializeField] private Characters m_character;
    [SerializeField] private GameObject m_interactText;

    [Header("Dialogue")]
    [SerializeField] private DialogueScene m_scene;
    [SerializeField] private GameObject m_dialogueCamera;

    private bool _isTalking;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent.CompareTag("Player"))
            return;


        m_interactText.SetActive(true);

        PlayerInputSingleton.Instance.Actions["Interact"].performed += StartDialogue;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.parent.CompareTag("Player"))
            return;

        m_interactText.SetActive(false);

        _isTalking = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        DialogueManager.Instance.EndDialogue();
        PlayerInputSingleton.Instance.Actions["Interact"].performed -= StartDialogue;
    }

    private void StartDialogue(InputAction.CallbackContext ctx)
    {
        if (_isTalking) return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        m_interactText.SetActive(false);
        DialogueManager.Instance.StartDialogue(m_scene.StartingDialogue);
        GameEvents.SetDialogueState(true);
        m_dialogueCamera.SetActive(true);

        _isTalking = true;
    }

    public void EndDialogue()
    {
        m_dialogueCamera.SetActive(false);

        if (gameObject.activeSelf)
            StartCoroutine(Wait());

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator Wait()
    {
        yield return null;

        GameEvents.SetDialogueState(false);

        _isTalking = false;
    }

    public void AddMember()
    {
        PartyManager.Instance.AddPartyMember(m_character);
        SaveSystem.Instance.CurrentSave.completedEvents.Add(uniqueID);
        gameObject.SetActive(false);
    }

    public override void SaveState(SaveData save)
    {
        //if (!save.completedEvents.Contains(uniqueID) && !gameObject.activeSelf)
        //    save.completedEvents.Add(uniqueID);
    }

    public override void LoadState(SaveData save)
    {
        m_interactText.SetActive(false);
        if (m_character == FindAnyObjectByType<CharController>().GetComponent<Player>().Name)
            gameObject.SetActive(false);
        else
        {
            if (save.completedEvents.Contains(uniqueID))
                gameObject.SetActive(false);
            else if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
    }
}
