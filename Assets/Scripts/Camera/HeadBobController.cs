using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HeadBobController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharController character;
    [SerializeField] private Transform cameraPoint;   // il nodo che si muove sull'asse Z

    [Header("Bobbing Settings")]
    [SerializeField] private float walkAmplitude = 0.05f;
    [SerializeField] private float walkFrequency = 8f;
    [SerializeField] private float runAmplitude = 0.1f;
    [SerializeField] private float runFrequency = 11f;

    [Header("Return Speed")]
    [SerializeField] private float smooth = 8f;

    private Vector3 _defaultLocalPos;
    private float _timer;
    private Animator _animator;

    private void Start()
    {
        if (cameraPoint == null)
        {
            Debug.LogError("HeadBobbing3P: manca la reference a cameraPoint.");
            enabled = false;
            return;
        }

        _defaultLocalPos = cameraPoint.localPosition;
        _animator = character.transform.Find("Model").GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (character == null || cameraPoint == null) return;

        bool grounded = IsGrounded();
        float speed = GetSpeed();
        bool running = IsRunning();

        if (!grounded || speed < 0.1f)
        {
            // Torna alla posizione base
            cameraPoint.localPosition = Vector3.Lerp(
                cameraPoint.localPosition,
                _defaultLocalPos,
                Time.deltaTime * smooth
            );
            return;
        }

        float amp = running ? runAmplitude : walkAmplitude;
        float freq = running ? runFrequency : walkFrequency;

        _timer += Time.deltaTime * freq;

        float bobY = Mathf.Sin(_timer) * amp;
        float bobZ = Mathf.Cos(_timer * 0.5f) * amp * 0.5f;

        Vector3 target = _defaultLocalPos + new Vector3(0, bobY, bobZ);

        cameraPoint.localPosition = Vector3.Lerp(
            cameraPoint.localPosition,
            target,
            Time.deltaTime * smooth
        );
    }

    // ----------------------------
    // Access a CharController fields
    // ----------------------------
    private bool IsGrounded()
    {
        // il tuo metodo è privato, quindi uso l’animator:
        return _animator.GetBool("IsGrounded");
    }

    private bool IsRunning()
    {
        return _animator.GetBool("IsRunning");
    }

    private float GetSpeed()
    {
        // Uso la magnitude del currentSpeed leggendo l’animator
        float x = _animator.GetFloat("X");
        float y = _animator.GetFloat("Y");
        return new Vector2(x, y).magnitude;
    }

    public void SetCharacter(CharController chara)
    {
        character = chara;
    }
}
