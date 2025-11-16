using UnityEngine;

public class Actions : MonoBehaviour
{
    [SerializeField] private Animator m_animator;
    public void Action1()
    {
        Debug.Log("Action 1");
        m_animator.CrossFade("Animation1", 0.05f);
    }
    public void Action2()
    {
        Debug.Log("Action 2");
        m_animator.CrossFade("Animation2", 0.05f);
    }
    public void Action3()
    {
        Debug.Log("Action 3");
        m_animator.CrossFade("Animation3", 0.05f);
    }
}
