using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CharCtrl : MonoBehaviour
{
    private const float MIN_MOVE_SPEED = 0.1f;

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private CharacterController _characterController;



    private Vector3 _currentSpeed;
    private Vector3 _wantedSpeed;

    private float _speedMagnitude;

    private float _vertical;
    private float _horizontal;




    private void Start()
    {
        _speedMagnitude = _walkSpeed;
        PlayerInputSingleton.Instance.Actions["Move"].performed += OnMoveInput;
        PlayerInputSingleton.Instance.Actions["Sprint"].started += OnSprintStart;
        PlayerInputSingleton.Instance.Actions["Sprint"].canceled += OnSprintEnd;
    }

    private void OnDestroy()
    {
        PlayerInputSingleton.Instance.Actions["Move"].performed -= OnMoveInput;
        PlayerInputSingleton.Instance.Actions["Sprint"].started -= OnSprintStart;
        PlayerInputSingleton.Instance.Actions["Sprint"].canceled -= OnSprintEnd;
    }




    private void Update()
    {
        _wantedSpeed.z = _vertical * _speedMagnitude;
        _wantedSpeed.x = _horizontal * _speedMagnitude;

        _currentSpeed = Vector3.MoveTowards(_currentSpeed, _wantedSpeed, _acceleration * Time.deltaTime);

        _characterController.SimpleMove(_currentSpeed);
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();

        Debug.Log($"Input Vector = {inputVector}");

        _horizontal = inputVector.x;
        _vertical = inputVector.y;
    }

    private void OnSprintStart(InputAction.CallbackContext context)
    {
        _speedMagnitude = _runSpeed;
    }

    private void OnSprintEnd(InputAction.CallbackContext context)
    {
        _speedMagnitude = _walkSpeed;
    }
}
