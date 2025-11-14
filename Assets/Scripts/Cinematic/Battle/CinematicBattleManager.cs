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
        // 1. Collega l’evento "StartAttack" della timeline all’attacco effettivo
        UnityAction call = unit.Attack;
        BindSignalUtility.BindSingleCallSingleAsset(call, "StartAttack", unit.SignalReceiver);

        // 2. Salva la rotazione originale della unit (non del manager!)
        Quaternion originalRotation = unit.transform.rotation;

        // 3. Ruota la unit verso il target (solo sull’asse Y)
        if (unit.Target != null)
        {
            Vector3 dir = unit.Target.transform.position - unit.transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                unit.transform.rotation = Quaternion.LookRotation(dir);
        }

        // 4. Avvia la cinematica e attendi la fine
        yield return m_cinematicManager.PlayCinematicCoroutine(unit.BaseAttack, unit.AttackCinematic);

        // 5. Ripristina la rotazione originale
        unit.transform.rotation = originalRotation;
    }

}
