using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    private const int MAX_PARTY_MEMBERS = 3;

    [SerializeField] private Player[] m_party = new Player[3];

    public static PartyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPartyMember(Player newMember)
    {
        if (PartySizeCheck())
            m_party.AddRange(m_party);
    }

    public void RemovePartyMember(Player removedMember)
    {
        for (int i = 0; i < m_party.Length; i++)
        {
            if (m_party[i] == removedMember)
            {
                m_party[i] = null;

                // opzionale ma consigliato → compattare l’array
                for (int j = i; j < m_party.Length - 1; j++)
                    m_party[j] = m_party[j + 1];

                // libera l’ultimo slot dopo lo shift
                m_party[m_party.Length - 1] = null;

                return;
            }
        }
    }

    private bool PartySizeCheck()
    {
        if (m_party.Length >= 3)
            return false;

        return true;
    }
}
