using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    public SaveData CurrentSave { get; private set; }

    private string savePath;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            SaveGame();
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
            ReloadSceneState();
        }
    }

    public void NewGame()
    {
        CurrentSave = new SaveData();
        CurrentSave.creationDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        CurrentSave.totalPlayTime = 0;
    }

    public void SaveGame()
    {
        if (CurrentSave == null)
        {
            Debug.LogWarning("Nessun save attivo! Creo un nuovo save...");
            NewGame();
        }

        // Qui aggiorno tutti gli oggetti Saveable
        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>(true)
                                  .OfType<ISaveable>()
                                  .ToArray();

        foreach (var s in saveables)
        {
            s.SaveState(CurrentSave);
        }

        // Ora il JSON sarà pieno
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Salvataggio completato: {savePath}");
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Nessun file di salvataggio trovato. Creo un nuovo save...");
            NewGame();
            return;
        }

        string json = File.ReadAllText(savePath);
        CurrentSave = JsonUtility.FromJson<SaveData>(json);

        Debug.Log("Save caricato correttamente.");
    }

    private void ReloadSceneState()
    {
        var save = CurrentSave;
        var saveables = FindObjectsOfType<MonoBehaviour>(true);

        foreach (var s in saveables)
        {
            if (s is ISaveable saveable)
            {
                saveable.LoadState(save);
            }
        }
    }

}