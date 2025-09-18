using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnimationEvent
{
    [SerializeField] private string _eventName;
    public UnityEvent OnAnimationEvent;

    public string EventName => _eventName;
}
