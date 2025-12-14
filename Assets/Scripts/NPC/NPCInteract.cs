using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] private GameObject m_interactText;
    [SerializeField] private NovelScene m_scene;

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

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        NovelGUI.Instance.EndDialogue();
        PlayerInputSingleton.Instance.Actions["Interact"].performed -= StartDialogue;
    }

    private void StartDialogue(InputAction.CallbackContext ctx)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        m_interactText.SetActive(false);
        NovelGUI.Instance.StartDialogue(m_scene.StartingDialogue);
    }
}
