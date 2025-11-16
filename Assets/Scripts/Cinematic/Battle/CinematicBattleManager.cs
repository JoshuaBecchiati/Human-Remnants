using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicBattleManager : MonoBehaviour
{
    [SerializeField] private TimelineDatabase m_TimelineDB;
    [SerializeField] private CinemachineBrain m_cameraBrain;
    [SerializeField] private PlayableDirector m_director;

    [Header("Dependencies")]
    [SerializeField] private NewBattleManager m_newBattleManager;
    [SerializeField] private CinematicManager m_cinematicManager;

    private void OnEnable()
    {
        m_newBattleManager.OnStartAttack += PlayAttackCinematicCoroutine;

        BattleFlowManager.Instance.OnSetupBattle += SetupCinematicBattle;
    }

    private void OnDisable()
    {
        m_newBattleManager.OnStartAttack -= PlayAttackCinematicCoroutine;

        BattleFlowManager.Instance.OnSetupBattle -= SetupCinematicBattle;
    }

    public void SetupCinematicBattle(IReadOnlyList<GameObject> players, IReadOnlyList<GameObject> enemies)
    {
        TimelineAsset battleEnter = m_TimelineDB.GetTimeline("Battle enter");
        m_director.playableAsset = battleEnter;

        foreach (GameObject player in players)
        {
            player.GetComponent<AnimationPlayer>().CinematicController();

            BindSignal(player);
        }

        CinematicManager.PlayCinematic(battleEnter, m_director, m_cameraBrain);
    }

    private void BindSignal(GameObject unit)
    {
        SignalReceiver receiver = unit.GetComponentInChildren<SignalReceiver>();

        foreach (var output in m_director.playableAsset.outputs)
        {
            var track = output.sourceObject as TrackAsset;
            Object Binding = m_director.GetGenericBinding(track);

            if (track == null || Binding != null) continue;

            // Controllo più sicuro: è un SignalTrack?
            if (track.GetType().Name == "SignalTrack" || track is SignalTrack)
            {
                m_director.SetGenericBinding(track, receiver);
                break;
            }
        }
    }

    private IEnumerator PlayAttackCinematicCoroutine(UnitBase unit)
    {
        m_director = unit.AttackCinematic;

        BindSignal(unit.gameObject);

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
        yield return m_cinematicManager.PlayCinematicCoroutine(unit.BaseAttack, m_director);

        // 5. Ripristina la rotazione originale
        unit.transform.rotation = originalRotation;
    }

}
