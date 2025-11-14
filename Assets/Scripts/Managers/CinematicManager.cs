using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicManager : MonoBehaviour
{
    // --- Inspector ---
    [Header("Cinematic")]
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private CinemachineBrain m_cameraBrain;

    public void PlayCinematic(TimelineAsset timeline)
    {
        m_director.Stop();

        m_director.playableAsset = timeline;
        m_director.time = 0;

        foreach (var track in timeline.GetOutputTracks())
            if (track is CinemachineTrack cmTrack)
                m_director.SetGenericBinding(cmTrack, m_cameraBrain);

        m_director.Play();
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