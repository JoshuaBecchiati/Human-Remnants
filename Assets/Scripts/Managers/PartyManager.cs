using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class PartyManager : SaveableObject
{
    [Header("Party manager")]
    [SerializeField] private Transform m_partyParent;
    [SerializeField] private List<GameObject> m_party;
    [SerializeField] private List<GameObject> m_characters;

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

    public Player AddPartyMember(Characters newMember)
    {
        Player player = GetPlayerByName(newMember);

        if (player != null)
        {
            if (PartySizeCheck())
            {
                Transform activePlayer = FindAnyObjectByType<CharacterController>().transform;

                GameObject go = Instantiate(player.gameObject);
                go.transform.position = new Vector3(activePlayer.localPosition.x + 0.75f, activePlayer.localPosition.y + 0.2f, activePlayer.localPosition.z);
                go.transform.SetParent(m_partyParent, false);

                go.GetComponent<AllyController>().SetActivePlayer(activePlayer);
                go.GetComponent<NavMeshAgent>().enabled = true;
                m_party.Add(go);
                return go.GetComponent<Player>();
            }
        }

        return null;
    }

    public void RemovePartyMember(GameObject removedMember)
    {
        if (removedMember.GetComponent<Player>() == null) return;

        for (int i = 0; i < m_party.Count; i++)
        {
            if (m_party[i] == removedMember)
            {
                m_party[i] = null;

                // opzionale ma consigliato → compattare l’array
                for (int j = i; j < m_party.Count - 1; j++)
                    m_party[j] = m_party[j + 1];

                // libera l’ultimo slot dopo lo shift
                m_party[m_party.Count - 1] = null;

                return;
            }
        }
    }

    private bool PartySizeCheck()
    {
        if (m_party.Count >= 2)
            return false;

        return true;
    }

    public override void SaveState(SaveData save)
    {
        if (m_party.Count <= 0) return;

        save.party.Clear();

        for (int i = 0; i < m_party.Count; i++)
        {
            MemberData data = new();
            Player member = m_party[i].GetComponent<Player>();

            data.characterID = member.Name.ToString();
            data.hp = member.Health;

            save.party.Add(data);
        }
    }

    public override void LoadState(SaveData save)
    {
        if (save.party.Count <= 0) return;

        for (int i = 0; i < save.party.Count; i++)
        {
            if (Enum.TryParse<Characters>(save.party[i].characterID, out var characterID))
            {
                Player member = AddPartyMember(characterID);
                member.SetHealth(save.party[i].hp);
            }
        }
    }

    public Player GetPlayerByName(Characters newMember)
    {
        return m_characters
            .Select(go => go.GetComponent<Player>())   // prova a prendere il componente Player
            .FirstOrDefault(player => player != null && player.Name == newMember);
    }
}
