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
    [SerializeField] private BattleManager m_battleManager;
    [SerializeField] private CinematicManager m_cinematicManager;

    private void OnEnable()
    {
        m_battleManager.OnStartAttack += PlayAttackCinematicCoroutine;

        BattleFlowManager.Instance.OnSetupBattle += SetupCinematicBattle;
    }

    private void OnDisable()
    {
        m_battleManager.OnStartAttack -= PlayAttackCinematicCoroutine;

        BattleFlowManager.Instance.OnSetupBattle -= SetupCinematicBattle;
    }

    public void SetupCinematicBattle(IReadOnlyList<GameObject> players, IReadOnlyList<GameObject> enemies)
    {
        TimelineAsset battleEnter = m_TimelineDB.GetTimeline("Battle enter");
        m_director.playableAsset = battleEnter;

        foreach (GameObject player in players)
        {
            player.GetComponentInChildren<AnimationPlayer>().CinematicController();
            SignalReceiver receiver = player.GetComponentInChildren<SignalReceiver>();

            BindSignal(receiver, m_director);
        }

        CinematicManager.PlayCinematic(battleEnter, m_director, m_cameraBrain);
    }

    private void BindSignal(SignalReceiver receiver, PlayableDirector director)
    {
        foreach (var output in director.playableAsset.outputs)
        {
            var track = output.sourceObject as TrackAsset;
            Object Binding = director.GetGenericBinding(track);

            if (track == null || Binding != null) continue;

            // Controllo più sicuro: è un SignalTrack?
            if (track.GetType().Name == "SignalTrack" || track is SignalTrack)
            {
                director.SetGenericBinding(track, receiver);
                break;
            }
        }
    }

    private void BindAnimation(Animator animator, PlayableDirector director)
    {
        foreach (var output in director.playableAsset.outputs)
        {
            var track = output.sourceObject as TrackAsset;
            Object Binding = director.GetGenericBinding(track);

            if (track == null) continue;

            // Controllo più sicuro: è un SignalTrack?
            if (track.GetType().Name == "AnimationTrack" || track is AnimationTrack)
            {
                director.SetGenericBinding(track, animator);
                break;
            }
        }
    }

    //private IEnumerator PlayAttackCinematicCoroutine(UnitBase unit)
    //{
    //    if (unit == null)
    //        yield break;

    //    GameObject attackPrefab = unit.AttackDatas.Find(n => n.attackName == "Base attack").attackPrefab;
    //    GameObject attack = Instantiate(attackPrefab, unit.transform.parent);

    //    PlayableDirector director = attack.GetComponent<PlayableDirector>();
    //    TimelineAsset attckTimeLine = director.playableAsset as TimelineAsset;

    //    BindAnimation(unit.Animator, director);
    //    BindSignal(unit.gameObject.GetComponent<SignalReceiver>(), director);

    //    // 2. Salva la rotazione originale della unit (non del manager!)
    //    Quaternion originalRotation = unit.gameObject.transform.parent.rotation;

    //    // 3. Ruota la unit verso il target (solo sull’asse Y)
    //    if (unit.Target != null)
    //    {
    //        Vector3 dir = unit.Target.gameObject.transform.parent.position - unit.gameObject.transform.position;
    //        dir.y = 0f;
    //        if (dir != Vector3.zero)
    //            unit.gameObject.transform.parent.rotation = Quaternion.LookRotation(dir);
    //    }

    //    // 4. Avvia la cinematica e attendi la fine
    //    yield return m_cinematicManager.PlayCinematicCoroutine(attckTimeLine, director);

    //    // 5. Ripristina la rotazione originale
    //    unit.gameObject.transform.parent.rotation = originalRotation;

    //    Destroy(attack);
    //}

    private IEnumerator PlayAttackCinematicCoroutine(UnitBase unit, AttackData attackData)
    {
        if (unit == null || attackData == null)
            yield break;

        GameObject attack = Instantiate(attackData.Animation, unit.transform.parent);

        PlayableDirector director = attack.GetComponent<PlayableDirector>();
        TimelineAsset attckTimeLine = director.playableAsset as TimelineAsset;

        BindAnimation(unit.Animator, director);
        BindSignal(unit.gameObject.GetComponent<SignalReceiver>(), director);

        // 2. Salva la rotazione originale della unit (non del manager!)
        Quaternion originalRotation = unit.gameObject.transform.parent.rotation;

        // 3. Ruota la unit verso il target (solo sull’asse Y)
        if (unit.Target != null)
        {
            Vector3 dir = unit.Target.gameObject.transform.parent.position - unit.gameObject.transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                unit.gameObject.transform.parent.rotation = Quaternion.LookRotation(dir);
        }

        // 4. Avvia la cinematica e attendi la fine
        yield return m_cinematicManager.PlayCinematicCoroutine(attckTimeLine, director);

        // 5. Ripristina la rotazione originale
        unit.gameObject.transform.parent.rotation = originalRotation;

        Destroy(attack);
    }
}