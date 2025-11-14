using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class CharController : MonoBehaviour
{
    // --- Constant ---
    private const float MIN_MOVE_SPEED = 0.1f;
    private const float WALK_VALUE = 0.5f;
    private const float RUN_VALUE = 1.0f;
    private const float STOP_VALUE = 0.0f;

    // --- Inspector ---
    [Header("Jump")]
    [SerializeField] private float m_jumpForce = 3f;
    [SerializeField] private float m_gravity = 15f;
    [SerializeField] private LayerMask m_groundMask;

    [Header("Camera")]
    [SerializeField] private float m_cameraSpeed = 320f;
    [SerializeField] private Transform m_cameraPivot;

    [Header("Animation")]
    [SerializeField] private Animator m_animator;
    [SerializeField] private CharacterController m_characterController;
    [SerializeField] private float m_animationTransition;

    // --- Private ---
    private Vector3 _currentSpeed;
    private Vector3 _wantedSpeed;

    private float _horizontal;
    private float _vertical;
    private float _verticalVelocity = 0f;
    private float _speedMagnitude;

    private bool _isJumping;
    private bool _isClimbing;
    private bool _isRunning;
    private bool _isGrounded;
    private bool _isMoving;

    #region Unity methods
    // Start is called before the first frame update

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

    private void OnAnimatorMove()
    {
        Vector3 motion = m_animator.deltaPosition;
        motion.y = 0;
        m_characterController.Move(motion);
        transform.rotation = m_animator.rootRotation;
    }
    #endregion

    #region Movement logic
    protected void HandleMovement()
    {
        // MOVIMENTO WASD
        // Raccolgo inizializzo i dati in input
        _wantedSpeed.z = _vertical * _speedMagnitude;
        _wantedSpeed.x = _horizontal * _speedMagnitude;

        // Se mi sto muovendo la camera si deve spostare
        if (Mathf.Abs(_horizontal) > MIN_MOVE_SPEED || Mathf.Abs(_vertical) > MIN_MOVE_SPEED)
            OrientCharToCamera();

        // Gestione della gravità
        if (!_isClimbing)
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

        _currentSpeed = Vector3.MoveTowards(_currentSpeed, _wantedSpeed, m_animationTransition * Time.deltaTime);
        UpdateAnimator();

        // Movimento effettivo
        m_characterController.Move(m_cameraPivot.TransformDirection(_currentSpeed) * Time.deltaTime);
        MovementSounds.UpdateMovementState(_isRunning, _currentSpeed);
    }
    #endregion

    #region Climbing logic
    private void HandleClimbing()
    {
        Vector3 direction = Quaternion.Euler(0.0f, transform.eulerAngles.y, 0.0f) * Vector3.forward;

        float maxDistance = .5f;
        if (!_isClimbing)
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

        if (_isClimbing)
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
    #endregion

    #region Move logic
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // Raccolto i valori (da -1 a 1) dell'input action
        Vector2 input = context.ReadValue<Vector2>();

        _isMoving = true;

        // Assegno i valori raccolti
        _horizontal = input.x;
        _vertical = input.y;
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _isMoving = false;

        _horizontal = 0f;
        _vertical = 0f;
    }
    #endregion

    #region Jump logic
    private void OnJumpInput(InputAction.CallbackContext context)
    {
        if (IsGrounded())
        {
            m_animator.SetTrigger("Jump");
            _isJumping = true;
            _verticalVelocity = Mathf.Sqrt(2 * m_jumpForce * m_gravity);
        }
    }

    private bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position, 0.3f, m_groundMask)) return true;

        return false;
    }
    #endregion

    #region Run logic
    protected virtual void OnSprintStarted(InputAction.CallbackContext context)
    {
        if (!IsGrounded()) return;
        _speedMagnitude = RUN_VALUE;
        _isRunning = true;
    }

    protected virtual void OnSprintCanceled(InputAction.CallbackContext context)
    {
        if (!IsGrounded()) return;
        _speedMagnitude = WALK_VALUE;
        _isRunning = false;
    }
    #endregion

    #region Animator
    private void UpdateAnimator()
    {
        if (Mathf.Abs(m_animator.GetFloat("X")) < 0.005f && Mathf.Abs(m_animator.GetFloat("Y")) < 0.005f && !_isMoving)
        {
            _currentSpeed = Vector3.zero;
        }

        m_animator.SetFloat("X", _currentSpeed.x);
        m_animator.SetFloat("Y", _currentSpeed.z);
    }
    #endregion

    #region Camera orientation logic
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
    #endregion
}
