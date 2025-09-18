using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventState : StateMachineBehaviour
{
    [SerializeField] private string _eventName;
    [Range(0f, 1f), SerializeField] private float triggerTime;

    private bool _isTriggered;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isTriggered = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1f;

        if (!_isTriggered && currentTime >= triggerTime)
        {
            NotifyReciver(animator);
            _isTriggered = true;
        }
    }

    private void NotifyReciver(Animator animator)
    {
        AnimationEventReceiver receiver = animator.GetComponent<AnimationEventReceiver>();
        if (receiver != null)
        {
            receiver.OnAnimationEventTriggered(_eventName);
        }
    }
}
