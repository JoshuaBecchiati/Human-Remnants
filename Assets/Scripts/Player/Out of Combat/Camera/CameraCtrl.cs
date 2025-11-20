using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCtrl : MonoBehaviour
{
    // MUOVERE LA CAMERA
    // Utilizzare il follow-at
    // La camera di chinemachine ruota in base a dove sta guardando il follow at

    // Input per raccogliere i dati per la camera

    [SerializeField] private Transform _target;
    [SerializeField] private Transform _elevation;
    [SerializeField] private Transform _cameraPoint;

    [SerializeField] private Vector3 _offset;

    [SerializeField] private LayerMask _collisionMask;

    [SerializeField] private float _horizontalSpeed;
    [SerializeField] private float _verticalSpeed;
    [SerializeField] private float _maxElevation = 90f;
    [SerializeField] private float _minElevation = -90f;
    [SerializeField] private float _desiredArmLenght = 2;

    [SerializeField] private bool _invertMouse = false;

    private float _horizontalRotation;
    private float _verticalRotation;
    private float _currentArmLenght;
    private float _xRotation;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        _horizontalRotation = mouseDelta.x * _horizontalSpeed * Time.deltaTime;
        _verticalRotation = mouseDelta.y * _verticalSpeed * Time.deltaTime * (_invertMouse ? 1 : -1);

        // Rotazione orizzontale del corpo
        transform.Rotate(Vector3.up, _horizontalRotation);

        // Calcola e limita la rotazione verticale
        _xRotation += _verticalRotation;
        _xRotation = Mathf.Clamp(_xRotation, _minElevation, _maxElevation);

        _elevation.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        IsCameraOccluded();
    }

    private void OnValidate()
    {
        if (_target == null) return;
        transform.position = GetWantedPosition();
    }
    private void LateUpdate()
    {
        if (_target == null) return;
        transform.position = GetWantedPosition();
    }

    private Vector3 GetWantedPosition()
    {
        return _target.position + _offset;
    }

    private void IsCameraOccluded()
    {
        if (_target == null) return;

        RaycastHit hit;
        Ray ray = new Ray(_target.position + _offset, _cameraPoint.position - (_target.position + _offset));
        
        if (Physics.SphereCast(ray, 0.25f, out hit, _desiredArmLenght, _collisionMask))
        {
            SetArmLength(hit.distance);
        }
        else
        {
            SetArmLength(_desiredArmLenght);
        }
    }

    private void SetArmLength(float lenght)
    {
        _currentArmLenght = lenght;
        _cameraPoint.localPosition = new Vector3(0, 0, -lenght);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
