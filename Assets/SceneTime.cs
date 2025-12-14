using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SceneTime : MonoBehaviour
{
    [SerializeField] private Transform m_partyParent;
    [SerializeField] private List<GameObject> m_CharactersPrefab;
    
    private float _timePlay;

    public float TimePlay => _timePlay;

    public static SceneTime Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        GameObject go = Instantiate(FindCharacterPrefab());

        SaveSystem.Instance.LoadGame();

        go.transform.SetParent(m_partyParent, false);

        FindObjectOfType<CameraCtrl>().SetTarget(go.transform.Find("Model"));
        FindObjectOfType<HeadBobController>().SetCharacter(go.GetComponent<CharController>());
    }

    private void Start()
    {
        if (SaveSystem.Instance.CurrentSave != null)
            _timePlay = SaveSystem.Instance.CurrentSave.totalPlayTime;
    }

    private void Update()
    {
        if (SaveSystem.Instance.CurrentSave != null)
            _timePlay += Time.deltaTime;
    }

    public GameObject FindCharacterPrefab()
    {
        string targetID = SaveSystem.Instance.CurrentSave.player.characterID;

        foreach (GameObject prefab in m_CharactersPrefab)
        {
            Transform transformPF = prefab.transform;
            if (transformPF == null) continue;

            Player p = transformPF.GetComponent<Player>();
            if (p == null) continue;

            if (p.Name.ToString() == targetID)
            {
                Debug.Log("Prefab trovato: " + prefab.name);
                return prefab;
            }
        }

        Debug.LogWarning("Nessun prefab corrisponde all'ID " + targetID);
        return null;
    }
}
