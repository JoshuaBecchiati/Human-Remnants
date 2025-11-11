using UnityEngine;

[CreateAssetMenu(fileName = "FootstepSurface", menuName = "Audio/Footstep Surface")]
public class FootstepSurface : ScriptableObject
{
    public AudioClip[] walkClips;
    public AudioClip[] runClips;
}