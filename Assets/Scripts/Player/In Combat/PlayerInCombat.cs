using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInCombat : UnitBase
{
    [Header("Abilities")]
    [SerializeField] private List<AbilityData> _AbilitiesData = new();
    [SerializeField] private CinemachineVirtualCamera _VirtualCamera;
    [SerializeField] private Transform m_targetCamera;
    
    private UIBattleManager m_uiBattleManager;

    protected override void Awake()
    {
        base.Awake();
        m_targetCamera = GameObject.Find("Enemy Side").transform;
    }

    public void Init(UIBattleManager uIBattleManager)
    {
        m_uiBattleManager = uIBattleManager;
    }

    public override void StartTurn()
    {
        base.StartTurn();

        BattleCameraManager.Instance.SetBattleCamera(_VirtualCamera);
        _VirtualCamera.LookAt = m_targetCamera;

        if (_AbilitiesData != null)
            foreach (AbilityData ability in _AbilitiesData)
                ability.CharchingAbility();

        if (m_uiBattleManager != null)
            m_uiBattleManager.CreateAbilityUI(_AbilitiesData);
    }

    public IReadOnlyList<AbilityData> GetAbilities()
    {
        return _AbilitiesData;
    }
}
