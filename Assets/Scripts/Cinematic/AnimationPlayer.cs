using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private RuntimeAnimatorController m_CombatController;
    [SerializeField] private RuntimeAnimatorController m_CinematicController;

    private void OnValidate()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    public void StartEntrance()
    {
        m_animator.CrossFade("StartEntrance", 0.05f);
    }

    public void ReadyPose()
    {
        m_animator.CrossFade("ReadyPose", 0.05f);
    }

    public void CombatPose()
    {
        m_animator.CrossFade("CombatPose", 0.3f);
    }

    public void CombatController()
    {
        m_animator.runtimeAnimatorController = m_CombatController;
    }

    public void CinematicController()
    {
        m_animator.runtimeAnimatorController = m_CinematicController;
    }
}
