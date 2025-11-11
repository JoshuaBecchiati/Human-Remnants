using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AllyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_activePlayer;

    private void OnValidate()
    {
        if (!m_agent) m_agent = GetComponent<NavMeshAgent>();
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;
        m_animator.applyRootMotion = true;
    }

    private void Update()
    {
        if (!m_activePlayer) return;

        // Aggiorna destinazione
        m_agent.SetDestination(m_activePlayer.position);

        float distance = Vector3.Distance(transform.position, m_agent.destination);

        // Se siamo dentro lo stoppingDistance, ci fermiamo
        if (distance <= m_agent.stoppingDistance)
        {
            m_animator.SetFloat("X", 0, 0.2f, Time.deltaTime);
            m_animator.SetFloat("Y", 0, 0.2f, Time.deltaTime);
            return;
        }

        // Direzione verso il target
        Vector3 dir = (m_agent.steeringTarget - transform.position);
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            Vector3 localDir = transform.InverseTransformDirection(dir.normalized);
            m_animator.SetFloat("X", localDir.x, 0.2f, Time.deltaTime);
            m_animator.SetFloat("Y", localDir.z, 0.2f, Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 360 * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        if (m_agent.enabled)
        {
            m_agent.nextPosition = m_animator.rootPosition;
            transform.position = m_agent.nextPosition;
        }
    }

    public void SetActivePlayer(Transform activePlayer)
    {
        m_activePlayer = activePlayer;
    }
}
