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

    [Header("Movement")]
    [SerializeField] protected float m_walkSpeed = 5f;
    [SerializeField] protected float m_runSpeed = 8f;
    [SerializeField] private float m_acceleration = 2f;

    [Header("Jump")]
    [SerializeField] private float m_jumpForce = 2.5f;
    [SerializeField] private float m_gravity = 9.81f;

    [Header("Camera")]
    [SerializeField] private float m_cameraSpeed = 5f;
    [SerializeField] private Transform m_cameraPivot;

    [Header("Others")]
    [SerializeField] CharacterController m_characterController;
    [SerializeField] Animator m_animator;


    private bool _isJumping;
    private bool _isClimbing;
    private float _verticalVelocity = 0f;

    /*
     *  Separare le variabili di velocità (_walkSpeed, _runSpeed, ecc.)
     *  serve a mantenere il codice ordinato e flessibile.
     *  Così, se in futuro vuoi introdurre rallentamenti
     *  (es. _slownessSpeed), puoi semplicemente cambiare
     *  il valore di _speedMagnitude senza modificare o complicare i metodi esistenti.
     */
    protected float _speedMagnitude;

    // Coordinate base di movimento
    private float _vertical;
    private float _horizontal;

    // Serve per avvicinarsi lentamente alla velocità massima voluta
    private Vector3 _currentSpeed;

    // Indica la velocità massima a cui si vuole andare
    private Vector3 _wantedSpeed;

    protected virtual void Start()
    {
        _speedMagnitude = m_walkSpeed;
        if(PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Move"].performed += OnMoveInput;
            PlayerInputSingleton.Instance.Actions["Jump"].performed += OnJumpInput;
            PlayerInputSingleton.Instance.Actions["Sprint"].started += OnSprintStarted;
            PlayerInputSingleton.Instance.Actions["Sprint"].canceled += OnSprintCanceled;
        }
    }

    protected virtual void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Move"].performed -= OnMoveInput;
            PlayerInputSingleton.Instance.Actions["Jump"].performed -= OnJumpInput;
            PlayerInputSingleton.Instance.Actions["Sprint"].started -= OnSprintStarted;
            PlayerInputSingleton.Instance.Actions["Sprint"].canceled -= OnSprintCanceled;
        }
    }

    private void OnEnable()
    {
        _speedMagnitude = m_walkSpeed;
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Move"].performed += OnMoveInput;
            PlayerInputSingleton.Instance.Actions["Jump"].performed += OnJumpInput;
            PlayerInputSingleton.Instance.Actions["Sprint"].started += OnSprintStarted;
            PlayerInputSingleton.Instance.Actions["Sprint"].canceled += OnSprintCanceled;
        }
    }

    private void OnDisable()
    {
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Move"].performed -= OnMoveInput;
            PlayerInputSingleton.Instance.Actions["Jump"].performed -= OnJumpInput;
            PlayerInputSingleton.Instance.Actions["Sprint"].started -= OnSprintStarted;
            PlayerInputSingleton.Instance.Actions["Sprint"].canceled -= OnSprintCanceled;
        }
    }

    protected void HandleMovement()
    {
        // MOVIMENTO WASD
        // Raccolgo inizializzo i dati in input
        _wantedSpeed.z = _vertical * _speedMagnitude;
        _wantedSpeed.x = _horizontal * _speedMagnitude;

        // Se mi sto muovendo la camera si deve spostare
        if (Mathf.Abs(_horizontal) > MIN_MOVE_SPEED || Mathf.Abs(_vertical) > MIN_MOVE_SPEED)
        {
            OrientCharToCamera();
        }

        // Calcolo per il movimento
        _currentSpeed = Vector3.Lerp(_currentSpeed, _wantedSpeed, m_acceleration * Time.deltaTime);

        // Gestione della gravità
        if(!_isClimbing)
        {
            if (m_characterController.isGrounded && !_isJumping)
            {
                _verticalVelocity = -1f;
            }
            else
            {
                // Calcolo della velocità verticale per far tornare il giocatore a terra
                _verticalVelocity += -m_gravity * Time.deltaTime;
                _isJumping = false;
            }
            _currentSpeed.y = _verticalVelocity;
        }
        HandleClimbing();



        // Movimento effettivo
        m_characterController.Move(m_cameraPivot.TransformDirection(_currentSpeed) * Time.deltaTime);

        UpdateAnimation(_currentSpeed.x, _currentSpeed.z);
    }

    private void HandleClimbing()
    {
        Vector3 direction = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f) * Vector3.forward;

        float maxDistance = .5f;
        if(!_isClimbing)
        {
            if (Physics.Raycast(transform.position + Vector3.down * .85f, direction, out RaycastHit hit, maxDistance))
            {
                if (hit.transform.CompareTag("Ladder"))
                {
                    GrabLadder();
                }
            }
        }
        else
        {
            if (Physics.Raycast(transform.position + Vector3.down * .85f, direction, out RaycastHit hit, maxDistance))
            {
                if (!hit.transform.CompareTag("Ladder"))
                {
                    DropLadder();
                    _verticalVelocity = 4f;
                }
            }
            else
            {
                DropLadder();
                _verticalVelocity = 4f;
            }
        }

        if(_isClimbing)
        {
            _currentSpeed.y = _vertical * _speedMagnitude;

            _currentSpeed.z = 0;
            _currentSpeed.x = 0;
            _verticalVelocity = 0f;
            _isJumping = false; 
        }
    }

    private void GrabLadder()
    {
        _isClimbing = true;
    }
    private void DropLadder()
    {
        _isClimbing = false;
    }

    private void UpdateAnimation(float x, float y)
    {
        m_animator.SetFloat("X", x);
        m_animator.SetFloat("Y", y);
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // Raccolto i valori (da -1 a 1) dell'input action
        Vector2 input = context.ReadValue<Vector2>();

        // Assegno i valori raccolti
        _horizontal = input.x;
        _vertical = input.y;
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (m_characterController.isGrounded)
        {
            m_animator.SetTrigger("Jump");
            _isJumping = true;
            _verticalVelocity = Mathf.Sqrt(2 * m_jumpForce * m_gravity);
        }
    }

    // Da spostare nello script della camera
    private void OrientCharToCamera()
    {
        Vector3 lookDirection = m_cameraPivot.forward;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            m_cameraSpeed * Time.deltaTime
        );
    }

    protected virtual void OnSprintStarted(InputAction.CallbackContext context)
    {
        _speedMagnitude = m_runSpeed;
    }

    protected virtual void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _speedMagnitude = m_walkSpeed;
    }
}
