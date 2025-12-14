using UnityEngine;
using UnityEngine.InputSystem;

public class CharController : MonoBehaviour
{
    // --- Constant ---
    private const float MIN_MOVE_SPEED = 0.1f;
    private const float WALK_VALUE = 0.5f;
    private const float RUN_VALUE = 1.0f;
    private const float FALL_DELAY = 0.1f;

    // --- Inspector ---
    [Header("Jump")]
    [SerializeField] private float m_jumpForce = 3f;
    [SerializeField] private float m_gravity = 16f;
    [SerializeField] private float m_jumpHorizontalSpeed = 8f;
    [SerializeField] private float m_groundedSphere = 0.25f;
    [SerializeField] private LayerMask m_groundMask;
    [SerializeField] private Transform m_groundCheck;

    [Header("Camera")]
    [SerializeField] private float m_cameraSpeed = 320f;
    [SerializeField] private Transform m_cameraPivot;

    [Header("Animation")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private float m_animationTransition = 20f;

    // --- Private ---
    private Vector3 _currentSpeed;
    private Vector3 _wantedSpeed;

    private float _horizontal;
    private float _vertical;
    private float _verticalVelocity = 0f;
    private float _speedMagnitude;
    private float _airTime;

    private bool _isJumping;
    private bool _isRunning;
    private bool _isMoving;
    private bool _isReallyFalling;

    #region Unity methods
    void Start()
    {
        _speedMagnitude = WALK_VALUE;

        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Move"].performed += OnMoveInput;
            PlayerInputSingleton.Instance.Actions["Move"].canceled += OnMoveCanceled;
            PlayerInputSingleton.Instance.Actions["Jump"].performed += OnJumpInput;
            PlayerInputSingleton.Instance.Actions["Sprint"].started += OnSprintStarted;
            PlayerInputSingleton.Instance.Actions["Sprint"].canceled += OnSprintCanceled;
        }

        if (!m_cameraPivot) m_cameraPivot = GameObject.FindGameObjectWithTag("Player camera").transform;
    }

    private void OnDestroy()
    {
        if (PlayerInputSingleton.Instance != null)
        {
            PlayerInputSingleton.Instance.Actions["Move"].performed -= OnMoveInput;
            PlayerInputSingleton.Instance.Actions["Move"].canceled -= OnMoveCanceled;
            PlayerInputSingleton.Instance.Actions["Jump"].performed -= OnJumpInput;
            PlayerInputSingleton.Instance.Actions["Sprint"].started -= OnSprintStarted;
            PlayerInputSingleton.Instance.Actions["Sprint"].canceled -= OnSprintCanceled;
        }
    }

    private void OnValidate()
    {
        if (!m_animator) m_animator = transform.Find("Model").GetComponent<Animator>();
        if (!m_characterController) m_characterController = transform.Find("Model").GetComponent<CharacterController>();
        if (!m_groundCheck) m_groundCheck = GameObject.Find("GroundCheck").transform;
    }

    private void Update()
    {
        HandleMovement();
        HandleGravity();

        MovementSounds.UpdateMovementState(_isRunning, _currentSpeed);

        ApplyFinalMovement();
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {
        if (IsGrounded())
        {
            _speedMagnitude = _isRunning ? RUN_VALUE : WALK_VALUE;
            m_animator.SetBool("IsRunning", _isRunning);
        }

        _wantedSpeed.z = _vertical * _speedMagnitude;
        _wantedSpeed.x = _horizontal * _speedMagnitude;

        if (Mathf.Abs(_horizontal) > MIN_MOVE_SPEED || Mathf.Abs(_vertical) > MIN_MOVE_SPEED)
            OrientCharToCamera();

        if (IsGrounded())
        {
            _currentSpeed = Vector3.MoveTowards(
                _currentSpeed,
                _wantedSpeed,
                m_animationTransition * Time.deltaTime
            );

            UpdateAnimator();
        }
        else
        {
            Vector3 airVelocity = _wantedSpeed * m_jumpHorizontalSpeed;
            airVelocity.y = _verticalVelocity;
            _currentSpeed = airVelocity;
        }
    }
    #endregion

    #region Gravity
    private void HandleGravity()
    {
        if (IsGrounded())
        {
            _airTime = 0f;
            _isReallyFalling = false;

            m_animator.SetBool("IsGrounded", true);
            m_animator.SetBool("IsFalling", false);
            m_animator.SetBool("IsJumping", false);

            if (_verticalVelocity < 0)
                _verticalVelocity = -2f; // stick to ground
        }
        else
        {
            _airTime += Time.deltaTime;
            _verticalVelocity -= m_gravity * Time.deltaTime;

            // salto: prima fase
            if (_verticalVelocity > 0)
            {
                m_animator.SetBool("IsJumping", true);
                m_animator.SetBool("IsFalling", false);
            }
            // caduta vera solo dopo 1 secondo
            else if (_airTime >= FALL_DELAY)
            {
                _isReallyFalling = true;
                m_animator.SetBool("IsJumping", false);
                m_animator.SetBool("IsFalling", true);
            }

            m_animator.SetBool("IsGrounded", false);
        }

        _currentSpeed.y = _verticalVelocity;
    }
    #endregion

    #region Jump
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            _isJumping = true;

            m_animator.SetBool("IsJumping", true);
            m_animator.SetBool("IsGrounded", false);
            m_animator.SetBool("IsFalling", false);

            _verticalVelocity = Mathf.Sqrt(2 * m_jumpForce * m_gravity);
        }
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(m_groundCheck.position, m_groundedSphere, m_groundMask) && _verticalVelocity <= 0.01f;
    }
    #endregion

    #region Apply movement
    private void ApplyFinalMovement()
    {
        m_characterController.Move(
            m_cameraPivot.TransformDirection(_currentSpeed) * Time.deltaTime
        );
    }
    #endregion

    #region Input
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        _isMoving = true;
        _horizontal = input.x;
        _vertical = input.y;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _isMoving = false;
        _horizontal = 0f;
        _vertical = 0f;
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        _isRunning = true;

        if (!IsGrounded()) return;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        _isRunning = false;

        if (!IsGrounded()) return;
    }
    #endregion

    #region Animator + rotation
    private void UpdateAnimator()
    {
        if (Mathf.Abs(m_animator.GetFloat("X")) < 0.005f &&
            Mathf.Abs(m_animator.GetFloat("Y")) < 0.005f &&
            !_isMoving)
        {
            _currentSpeed = Vector3.zero;
        }

        if (Mathf.Abs(m_animator.GetFloat("X")) > 1f && _isMoving)
        {
            _currentSpeed.x = 1f;
        }

        if (Mathf.Abs(m_animator.GetFloat("Y")) > 1f && _isMoving)
        {
            _currentSpeed.z = 1f;
        }

        m_animator.SetFloat("X", _currentSpeed.x);
        m_animator.SetFloat("Y", _currentSpeed.z);
    }

    private void OrientCharToCamera()
    {
        Vector3 lookDirection = m_cameraPivot.forward;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.Find("Model").rotation = Quaternion.RotateTowards(
            transform.Find("Model").rotation,
            targetRotation,
            m_cameraSpeed * Time.deltaTime
        );
    }
    #endregion
}
