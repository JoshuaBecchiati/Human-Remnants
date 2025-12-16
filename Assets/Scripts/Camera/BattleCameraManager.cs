using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_battleCamera;
    [SerializeField] private CinemachineVirtualCamera m_enemyCamera;
    [SerializeField] private CinemachineVirtualCamera m_playerCamera;
    [SerializeField] private CinemachineVirtualCamera m_viewCamera;

    private List<CinemachineVirtualCamera> _allCameras = new();

    public static BattleCameraManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void SwitchCamera(CinemachineVirtualCamera activeCam)
    {
        foreach (CinemachineVirtualCamera cam in _allCameras)
            cam.Priority = (activeCam == cam) ? 10 : 0;
    }

    public void BattleCamera()
    {
        SwitchCamera(m_battleCamera);
    }

    public void EnemyCamera()
    {
        SwitchCamera(m_enemyCamera);
    }

    public void PlayerCamera()
    {
        SwitchCamera(m_playerCamera);
    }

    public void ViewCamera()
    {
        SwitchCamera(m_viewCamera);
    }

    public void SetBattleCamera(CinemachineVirtualCamera newBattleCamera)
    {
        if (m_battleCamera == newBattleCamera)
            return;

        // opzionale: spegni la vecchia
        if (m_battleCamera != null)
            m_battleCamera.gameObject.SetActive(false);

        m_battleCamera = newBattleCamera;

        if (!m_battleCamera.gameObject.activeSelf)
            m_battleCamera.gameObject.SetActive(true);

        RebuildCameraList();
    }

    private void RebuildCameraList()
    {
        _allCameras.Clear();

        if (m_battleCamera) _allCameras.Add(m_battleCamera);
        if (m_enemyCamera) _allCameras.Add(m_enemyCamera);
        if (m_playerCamera) _allCameras.Add(m_playerCamera);
        if (m_viewCamera) _allCameras.Add(m_viewCamera);
    }
}
