using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AllyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_agent;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_activePlayer;

    private bool _isOffMesh;

    private void OnValidate()
    {
        if (!m_agent) m_agent = GetComponent<NavMeshAgent>();
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_agent.updatePosition = false;
        m_agent.updateRotation = false;
        m_agent.autoTraverseOffMeshLink = false;
        m_animator.applyRootMotion = true;
    }

    private void Update()
    {
        if (!m_activePlayer) return;

        // Aggiorna destinazione
        if (Vector3.Distance(m_agent.destination, m_activePlayer.position) > 0.1f)
            m_agent.SetDestination(m_activePlayer.position);

        /*
         * Raycast ai piedi
         * Se null air,
         * 
         */

        if (m_agent.isOnOffMeshLink && !_isOffMesh)
        {
            OffMeshLinkData link = m_agent.currentOffMeshLinkData;

            if (Vector3.Distance(transform.position, link.startPos) < 0.5f)
            {
                _isOffMesh = true;
                StartCoroutine(DoOffMeshLink(link));
            }
        }
        else
        {
            _isOffMesh = false;
            float distance = Vector3.Distance(transform.position, m_agent.destination);

            // Se siamo dentro lo stoppingDistance, ci fermiamo
            if (distance <= m_agent.stoppingDistance)
            {
                m_animator.SetFloat("X", 0, 0.2f, Time.deltaTime);
                m_animator.SetFloat("Y", 0, 0.2f, Time.deltaTime);
                return;
            }

            // Direzione verso il target
            Vector3 dir = m_agent.steeringTarget - transform.position;
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

        //MovementSounds.UpdateMovementState();
    }

    private IEnumerator DoOffMeshLink(OffMeshLinkData link)
    {
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, link.startPos, Time.deltaTime);
            Vector3 dir = (link.endPos - link.startPos).normalized;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 180 * Time.deltaTime);

            bool isRotationGood = Vector3.Dot(dir, transform.forward) > 0.8f;
            if (isRotationGood) break;

            yield return null;
        }

        m_animator.CrossFade("Jump", 0f);

        float time = 0.5f;
        float totaltTime = time;

        while (time > 0)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            Vector3 goal = Vector3.Lerp(link.startPos, link.endPos, 1 - time / totaltTime);
            float elapsedTime = totaltTime - time;
            transform.position = elapsedTime > 0.3f ? goal : Vector3.Lerp(transform.position, goal, elapsedTime / 0.3f);
            yield return null;
        }

        transform.position = link.endPos;
        m_agent.CompleteOffMeshLink();
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
