using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        MenuManager.Instance.Back();
    }
}
