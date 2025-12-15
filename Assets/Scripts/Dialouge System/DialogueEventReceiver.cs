using UnityEngine;
using System;
using UnityEngine.Events;


public class DialogueEventReceiver : MonoBehaviour
{
    [Serializable]
    private class DialogueEventResponsePair
    {
        [SerializeField] private DialogueEvent _dialogueEvent;

        public UnityEvent response;

        private void FireResponse()
        {
            response.Invoke();
        }

        public void Initialize()
        {
            _dialogueEvent.OnDialogueEvent += FireResponse;
        }
        public void Deinitialize()
        {
            _dialogueEvent.OnDialogueEvent -= FireResponse;
        }
    }

    [SerializeField] private DialogueEventResponsePair[] _eventResponsePairs;

    private void Start()
    {
        foreach (var pair in _eventResponsePairs)
        {
            pair.Initialize();
        }
    }

    private void OnDestroy()
    {
        foreach (var pair in _eventResponsePairs)
        {
            pair.Deinitialize();
        }
    }
}