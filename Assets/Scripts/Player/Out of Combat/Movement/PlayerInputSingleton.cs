using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSingleton : MonoBehaviour
{
    // Instanza statica only read del PlayerInputSingleton
    public static PlayerInputSingleton Instance { get; private set; }

    [SerializeField] private PlayerInput _inputs;
    public InputActionAsset Actions => _inputs.actions;


    private void Awake()
    {
        // Singleton
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ExploreInput();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SwitchActionMap(string actionMap)
    {
        _inputs.SwitchCurrentActionMap(actionMap);
    }

    public void ExploreInput()
    {
        _inputs.actions.Disable();
        SwitchActionMap("Explore");
        _inputs.actions.Enable();
    }

    public void CombatInput()
    {
        _inputs.actions.Disable();
        SwitchActionMap("Combat");
        _inputs.actions.Enable();
    }
}