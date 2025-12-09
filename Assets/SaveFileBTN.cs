using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveFileBTN : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- Inspector ---
    //[SerializeField] private string m_normalTrigger = "Normal";
    [SerializeField] private string m_highlightedTrigger = "Highlighted";
    [SerializeField] private string m_dehighlightedTrigger = "Dehighlighted";

    // --- Private ---
    private Animator _animator;

    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animator.SetTrigger(m_highlightedTrigger);
        VolumeManager.Instance.PlayUISaveHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animator.SetTrigger(m_dehighlightedTrigger);
        VolumeManager.Instance.PlayUISaveHover();
    }
}
