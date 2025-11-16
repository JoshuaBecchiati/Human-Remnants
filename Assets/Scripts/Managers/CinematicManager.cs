using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicManager : MonoBehaviour
{
    // --- Inspector ---
    [Header("Cinematic")]
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private CinemachineBrain m_cameraBrain;

    public static void PlayCinematic(TimelineAsset timeline, PlayableDirector director, CinemachineBrain cameraBrain)
    {
        director.Stop();

        director.playableAsset = timeline;
        director.time = 0;

        foreach (var track in timeline.GetOutputTracks())
            if (track is CinemachineTrack cmTrack)
                director.SetGenericBinding(cmTrack, cameraBrain);

        director.Play();
    }

    public Coroutine PlayCinematicCoroutine(TimelineAsset timeline, PlayableDirector director)
    {
        return StartCoroutine(PlayAndWait(timeline, director));
    }

    private IEnumerator PlayAndWait(TimelineAsset timeline, PlayableDirector director)
    {
        bool finished = false;

        void DirectorStopped(PlayableDirector dir)
        {
            director.stopped -= DirectorStopped;
            finished = true;
        }

        director.stopped += DirectorStopped;
        director.playableAsset = timeline;
        director.time = 0;

        foreach (var track in timeline.GetOutputTracks())
            if (track is CinemachineTrack cmTrack)
                director.SetGenericBinding(cmTrack, m_cameraBrain);

        director.Play();

        yield return new WaitUntil(() => finished);
    }

    public void Stop()
    {
        if (m_director.state == PlayState.Playing)
            m_director.Stop();
    }
}