using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInCombat : UnitBase
{
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

        List<AbilityData> abilities = AbilityManager.Instance.GetAbilites().ToList();
        BattleCameraManager.Instance.SetBattleCamera(_VirtualCamera);
        _VirtualCamera.LookAt = m_targetCamera;

        if (abilities != null)
            foreach (AbilityData ability in abilities)
                ability.CharchingAbility();

        if (m_uiBattleManager != null)
            m_uiBattleManager.CreateAbilityUI(abilities);
    }
}
