using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwitchCharacter : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_characters;
    [SerializeField] private int m_index = 0;
    [SerializeField] private CameraCtrl m_camera;

    private bool isSwitched;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            Switch();
    }

    private void Switch()
    {
        if (isSwitched)
            return;

        int switchIndex = m_index + 1;
        if (switchIndex >= m_characters.Count)
            switchIndex = 0;

        GameObject current = m_characters[m_index];
        GameObject next = m_characters[switchIndex];

        Player currentPlayer = current.GetComponent<Player>();
        CharacterController currentCC = current.GetComponent<CharacterController>();
        NavMeshAgent currentAgent = current.GetComponent<NavMeshAgent>();
        Animator currentAnimator = current.GetComponent<Animator>();
        AllyController currentAlly = current.GetComponent<AllyController>();

        Player nextPlayer = next.GetComponent<Player>();
        CharacterController nextCC = next.GetComponent<CharacterController>();
        NavMeshAgent nextAgent = next.GetComponent<NavMeshAgent>();
        AllyController nextAlly = next.GetComponent<AllyController>();

        currentPlayer.enabled = false;
        currentCC.enabled = false;
        nextCC.enabled = false;

        currentAgent.enabled = true;
        currentAnimator.Rebind();
        currentAnimator.Update(0f);
        currentAlly.enabled = true;
        currentAlly.SetActivePlayer(next.transform);

        Vector3 temp = current.transform.position;
        current.transform.position = next.transform.position;
        next.transform.position = temp;

        currentCC.enabled = true;
        nextCC.enabled = true;
        nextPlayer.enabled = true;
        nextAgent.enabled = false;
        nextAlly.enabled = false;

        m_camera.SetTarget(next.transform);

        isSwitched = true;
        m_index = switchIndex;

        StartCoroutine(ResetSwitch());
    }

    private IEnumerator ResetSwitch()
    {
        yield return new WaitForSeconds(0.5f);
        isSwitched = false;
    }
}
