using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    [SerializeField] private List<AnimationEvent> _animationEvents = new();

    public void OnAnimationEventTriggered(string eventName)
    {
        AnimationEvent matchingEvent = _animationEvents.Find(se => se.EventName == eventName);
        matchingEvent?.OnAnimationEvent?.Invoke();
    }
}
