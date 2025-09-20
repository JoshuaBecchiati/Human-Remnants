using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenu : MonoBehaviour
{
    [SerializeField] private GameObject m_nextMenu;
    [SerializeField] private GameObject m_previousMenu;

    public void NextMenu()
    {
        m_nextMenu.SetActive(true);
        m_previousMenu.SetActive(false);
    }
}
