using UnityEngine;

public class MovementSounds : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioSource m_audioSourceFootSteps;
    [SerializeField] private LayerSurface[] m_layerSurfaces;
    [SerializeField] private float m_minTimeBetweenSteps = 0.1f;

    private float _lastFootstepTime;
    private float _tempSpeed;
    private FootstepSurface _currentSurface;
    private static bool _isRunning;
    private static Vector3 _currentSpeed;

    #region Footstep sound

    public void PlayFootstepSound()
    {
        if (_currentSpeed.magnitude < 0.1f) return;
        if (Time.time - _lastFootstepTime < m_minTimeBetweenSteps) return;
        CheckSurfaceLayer(); // aggiorna _currentSurface in base al layer sotto i piedi
        if (_currentSurface == null) return;
        AudioClip[] clips = _isRunning ? _currentSurface.runClips : _currentSurface.walkClips;
        if (clips.Length == 0) return;

        m_audioSourceFootSteps.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        _lastFootstepTime = Time.time;
    }

    private void CheckSurfaceLayer()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            int layer = hit.collider.gameObject.layer;
            foreach (LayerSurface ls in m_layerSurfaces)
            {
                if (LayerMask.NameToLayer(ls.layerName) == layer)
                {
                    _currentSurface = ls.surface;
                    return;
                }
            }
        }

        // Default, se non trova nessun layer corrispondente
        _currentSurface = null;
    }

    public static void UpdateMovementState(bool isRunning, Vector3 speed)
    {
        _isRunning = isRunning;
        _currentSpeed = speed;
    }
    #endregion
}

[System.Serializable]
public struct LayerSurface
{
    public string layerName;
    public FootstepSurface surface;
}
