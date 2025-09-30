using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBattleManager : MonoBehaviour
{
    [SerializeField] private GameObject m_scene;

    private Camera _battleCamera;

    private void OnEnable()
    {
        GameEvents.OnBattleStart += StartBattle;
        GameEvents.OnBattleStart += EndBattle;
    }

    private void StartBattle()
    {

    }

    private void EndBattle()
    {

    }
}
