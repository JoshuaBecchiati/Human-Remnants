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
        Transform model = go.transform.Find("Model");
        FindObjectOfType<CameraCtrl>().SetTarget(model);
        FindObjectOfType<HeadBobController>().SetCharacter(model.GetComponent<CharController>());

        SaveSystem.Instance.LoadGame();

        go.transform.SetParent(m_partyParent, true);
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
            Transform model = prefab.transform.Find("Model");
            if (model == null) continue;

            Player p = model.GetComponent<Player>();
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
