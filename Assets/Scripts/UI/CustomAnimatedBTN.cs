using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomAnimatedBTN : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    // --- Inspector ---
    [SerializeField] protected string m_highlightedTrigger = "Highlighted";
    [SerializeField] protected string m_dehighlightedTrigger = "Dehighlighted";

    // --- Private ---
    protected Animator _animator;

    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.Play("Base", 0, 0f);
        _animator.Update(0f);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName(m_highlightedTrigger)) return;

        _animator.SetTrigger(m_dehighlightedTrigger);
        VolumeManager.Instance.PlayUISaveHover();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(m_dehighlightedTrigger) || stateInfo.IsName("Base"))
        {
            _animator.SetTrigger(m_highlightedTrigger);
            VolumeManager.Instance.PlayUISaveHover();
        }
        else
        {
            _animator.SetTrigger(m_dehighlightedTrigger);
            VolumeManager.Instance.PlayUISaveHover();
        }
    }
}
