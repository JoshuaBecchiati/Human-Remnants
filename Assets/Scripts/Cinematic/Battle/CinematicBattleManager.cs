using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicBattleManager : MonoBehaviour
{
    [SerializeField] private TimelineAsset m_battleEnterTimeline;

    [Header("Dependencies")]
    [SerializeField] private NewBattleManager m_newBattleManager;
    [SerializeField] private CinematicManager m_cinematicManager;

    private void OnEnable()
    {
        m_newBattleManager.OnStartBattleCinematic += SetupBattleIntro;
        m_newBattleManager.OnStartAttack += PlayAttackCinematicCoroutine;
    }

    private void OnDisable()
    {
        m_newBattleManager.OnStartBattleCinematic -= SetupBattleIntro;
        m_newBattleManager.OnStartAttack -= PlayAttackCinematicCoroutine;
    }

    public void SetupBattleIntro(List<UnitBase> units, SignalReceiver receiver)
    {
        foreach (UnitBase unit in units)
        {
            if (!unit.TryGetComponent(out AnimationTimeLine anim))
                continue;

            anim.CinematicController();

            BindSignalArgs args = anim.BattleEnterArgs(receiver);
            BindSignalUtility.BindMultiCallsToMultiAssets(args);
        }

        m_cinematicManager.PlayCinematic(m_battleEnterTimeline);
    }

    private IEnumerator PlayAttackCinematicCoroutine(UnitBase unit)
    {
        UnityAction call = unit.Attack;
        BindSignalUtility.BindSingleCallSingleAsset(call, "StartAttack", unit.SignalReceiver);

        // Play the cinematic and wait
        yield return m_cinematicManager.PlayCinematicCoroutine(unit.BaseAttack, unit.AttackCinematic);
    }
}
