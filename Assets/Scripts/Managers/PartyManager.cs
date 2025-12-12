using Unity.VisualScripting;
using UnityEngine;

public class PartyManager : SaveableObject
{
    private const int MAX_PARTY_MEMBERS = 3;

    [SerializeField] private GameObject[] m_party = new GameObject[3];

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

    public void AddPartyMember(GameObject newMember)
    {
        if (newMember.GetComponent<Player>() == null) return;

        if (PartySizeCheck())
            m_party.AddRange(m_party);
    }

    public void RemovePartyMember(GameObject removedMember)
    {
        if (removedMember.GetComponent<Player>() == null) return;

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

    public override void SaveState(SaveData save)
    {
        if (m_party.Length <= 0) return;

        save.party = m_party;
    }

    public override void LoadState(SaveData save)
    {
        if (save.party.Length <= 0) return;

        m_party = save.party;
    }
}
