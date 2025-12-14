using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] private GameObject m_interactText;
    [SerializeField] private GameObject m_dialogueCamera;
    [SerializeField] private NovelScene m_scene;
    [SerializeField] private UnityEvent m_endDialogue;

    private bool _isTalking;

    private void Start()
    {
        m_interactText.SetActive(false);
    }


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

        NovelGUI.Instance.EndDialogue();
        PlayerInputSingleton.Instance.Actions["Interact"].performed -= StartDialogue;
    }

    private void StartDialogue(InputAction.CallbackContext ctx)
    {
        if (_isTalking) return;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        m_interactText.SetActive(false);
        NovelGUI.Instance.StartDialogue(m_scene.StartingDialogue, m_endDialogue);
        GameEvents.SetDialogueState(true);
        m_dialogueCamera.SetActive(true);

        _isTalking = true;
    }

    public void EndDialogue()
    {
        m_dialogueCamera.SetActive(false);
        GameEvents.SetDialogueState(false);
        _isTalking = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
