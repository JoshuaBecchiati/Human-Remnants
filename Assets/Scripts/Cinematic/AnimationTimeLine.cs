using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Timeline;

public class AnimationTimeLine : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private RuntimeAnimatorController m_CombatController;
    [SerializeField] private RuntimeAnimatorController m_CinematicController;

    private SignalReceiver m_srCombat;

    public void StartEntrance()
    {
        m_animator.CrossFade("StartEntrance", 0.3f);
    }

    public void ReadyPose()
    {
        m_animator.CrossFade("ReadyPose", 0.3f);
    }

    public void CombatPose()
    {
        m_animator.CrossFade("CombatPose", 0.3f);
    }

    //public BindSignalArgs BattleEnterArgs(SignalReceiver srCombat)
    //{
    //    m_srCombat = srCombat;
    //    BindSignalArgs args = new
    //    (
    //        new UnityAction[] { StartEntrance, ReadyPose, CombatPose },
    //        new string[] { "StartEntrance", "ReadyPose", "CombatPose"},
    //        m_srCombat
    //    );
    //    return args;
    //}

    public void CombatController()
    {
        m_animator.runtimeAnimatorController = m_CombatController;
    }

    public void CinematicController()
    {
        m_animator.runtimeAnimatorController = m_CinematicController;
    }
}
