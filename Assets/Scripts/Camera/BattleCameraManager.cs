using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_battleCamera;
    [SerializeField] private CinemachineVirtualCamera m_enemyCamera;
    [SerializeField] private CinemachineVirtualCamera m_playerCamera;

    private List<CinemachineVirtualCamera> _allCameras;

    private void Awake()
    {
        _allCameras = new List<CinemachineVirtualCamera>
        {
            m_battleCamera,
            m_enemyCamera,
            m_playerCamera
        };
    }

    private void SwitchCamera(CinemachineVirtualCamera activeCam)
    {
        foreach (CinemachineVirtualCamera cam in _allCameras)
            cam.Priority = (activeCam == cam) ? 10 : 0;
    }

    public void BattleCamera() => SwitchCamera(m_battleCamera);
    public void EnemyCamera() => SwitchCamera(m_enemyCamera);
    public void PlayerCamera() => SwitchCamera(m_playerCamera);
}
