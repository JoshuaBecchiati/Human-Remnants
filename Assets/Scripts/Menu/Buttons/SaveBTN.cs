using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBTN : MonoBehaviour, IUIButton
{
    [SerializeField] private UIButtonType m_buttonType;
    public UIButtonType buttonType => m_buttonType;

    public void OnButtonPressed()
    {
        SaveSystem.Instance.SaveGame();
    }
}
