using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomAnimatedBTN : MonoBehaviour, IPointerClickHandler
{
    // --- Inspector ---
    [SerializeField] protected string m_highlightedTrigger = "Highlighted";
    [SerializeField] protected string m_dehighlightedTrigger = "Dehighlighted";
    [SerializeField] protected List<CustomAnimatedBTN> m_buttonsGroup;

    // --- Private ---
    protected Animator _animator;
    protected bool _isOpen;

    public bool IsOpen => _isOpen;

    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.Play("Base", 0, 0f);
        _animator.Update(0f);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName(m_dehighlightedTrigger) || stateInfo.IsName("Base"))
        {
            _animator.SetTrigger(m_highlightedTrigger);
            VolumeManager.Instance.PlayUISaveHover();

            CustomAnimatedBTN cbt = m_buttonsGroup.Find(o => o.IsOpen && o.gameObject != this);
            if (cbt != null)
            {
                cbt.SetOpenState(false);
                cbt.AnimationClose();
            }

            _isOpen = true;
        }
        else
        {
            AnimationClose();
        }
    }

    public void AnimationClose()
    {
        _animator.SetTrigger(m_dehighlightedTrigger);
        VolumeManager.Instance.PlayUISaveHover();

        _isOpen = false;
    }

    public void SetOpenState(bool state)
    {
        _isOpen = state;
    }
}
