using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXEventReceiver : MonoBehaviour
{
    [SerializeField] private Transform m_transform;
    [SerializeField] private Animator m_animator;

    [SerializeField] private AudioSource m_source;
    [SerializeField] private List<AudioClip> m_clips;
    [SerializeField] private List<GameObject> m_particles;

    private void OnValidate()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
    }

    public void OnHit()
    {
        Debug.Log("Hit");
    }

    public void OnDeath()
    {
        UnitBase unitBase = GetComponent<UnitBase>();
        StartCoroutine(FallCoroutine());
    }

    private IEnumerator FallCoroutine()
    {
        GameObject goPart = Instantiate(m_particles[0], gameObject.transform.parent);
        goPart.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(1f);

        Vector3 startPosition = m_transform.position;
        Vector3 endPosition = startPosition + Vector3.down * 3f;
        float duration = 5f; // tempo in secondi per scendere
        float elapsed = 0f;

        while (elapsed < duration)
        {
            m_transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        m_transform.position = endPosition; // assicura che arrivi esattamente
        StartCoroutine(WaitDespawn());
    }

    private IEnumerator WaitDespawn()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}