using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionSelector : MonoBehaviour
{
    private PlayerInputSingleton _input;
    private bool _confirmed;
    private bool _cancelled;

    private void Awake()
    {
        _input = PlayerInputSingleton.Instance;
    }
    public void StartConfirmAction(Action onConfirm, Action onCancel)
    {
        StartCoroutine(WaitForConfirmation(onConfirm, onCancel));
    }
    private IEnumerator WaitForConfirmation(Action onConfirm, Action onCancel)
    {
        _confirmed = false;
        _cancelled = false;

        yield return new WaitUntil(() =>
        {
            if (_input.Actions["Confirm"].triggered)
            {
                _confirmed = true;
                return true;
            }
            if (_input.Actions["Cancel"].triggered)
            {
                _cancelled = true;
                return true;
            }
            return false;
        });

        if (_confirmed)
        {
            onConfirm?.Invoke();
        }
        else if (_cancelled)
        {
            onCancel?.Invoke();
        }
    }

    //private void ConfirmAttack()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        _playerActionState = PlayerActionState.Acting;
    //        StartCoroutine(ExecuteAttack());
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        _playerActionState = PlayerActionState.Idle;
    //        CancelAttack();
    //    }
    //}

    //private void ConfirmAction()
    //{
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        _playerActionState = PlayerActionState.Acting;
    //        StartCoroutine(ExecuteItemUse());
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        _playerActionState = PlayerActionState.Idle;
    //        CancelItemAction();
    //    }
    //}
}
