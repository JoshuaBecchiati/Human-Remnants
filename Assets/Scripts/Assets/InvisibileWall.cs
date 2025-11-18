using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibileWall : MonoBehaviour
{
    [SerializeField] private Animator m_animator;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_animator.SetBool("Open", true);
        }
    }
}
