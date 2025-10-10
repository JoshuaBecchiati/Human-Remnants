using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] private GameObject m_BattleCinematic;
    [SerializeField] private PlayableDirector m_battleEnterTimeLine;

    public SignalReceiver m_signalBattleEnter;

    [Header("Dependencies")]
    [SerializeField] private NewBattleManager m_BattleManager;

    public static CinematicManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        m_BattleManager.OnStartBattle += BattleEnterCinematic;

        m_BattleCinematic.SetActive(false);
    }

    public void BattleEnterCinematic()
    {
        m_BattleCinematic.SetActive(true);
        if (m_battleEnterTimeLine.time != 0f)
            m_battleEnterTimeLine.time = 0f;

        m_battleEnterTimeLine.Play();
        StartCoroutine(SetFalse((float)m_battleEnterTimeLine.duration));
    }

    private IEnumerator SetFalse(float time)
    {
        yield return new WaitForSeconds(time);
        m_BattleCinematic.SetActive(false);
    }
}
