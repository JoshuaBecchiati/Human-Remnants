using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicTest : MonoBehaviour
{
    [SerializeField] private PlayableDirector m_director;
    [SerializeField] private GameObject m_prefab;
    [SerializeField] private Transform m_spawnpoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject instance = Instantiate(m_prefab, m_spawnpoint.position, m_spawnpoint.rotation);
            SignalReceiver receiver = instance.GetComponentInChildren<SignalReceiver>();
            Debug.Log("Receiver trovato: " + (receiver != null));

            foreach (var output in m_director.playableAsset.outputs)
            {
                var track = output.sourceObject as TrackAsset;
                Object Binding = m_director.GetGenericBinding(track);

                if (track == null || Binding != null) continue;

                // Controllo più sicuro: è un SignalTrack?
                if (track.GetType().Name == "SignalTrack" || track is SignalTrack)
                {
                    Debug.Log("Binding SignalTrack: " + output.streamName + " -> " + instance.name);
                    m_director.SetGenericBinding(track, receiver);
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
            m_director.Play();
    }
}
