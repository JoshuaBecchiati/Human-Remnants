using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    // --- Instance ---
    public static SaveSystem Instance { get; private set; }

    // --- Proprierties ---
    public SaveData CurrentSave { get; private set; }
    public string CurrentSavePath {  get; private set; }

    // --- Private ---
    private string _savesFolder;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _savesFolder = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(_savesFolder))
            Directory.CreateDirectory(_savesFolder);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            SaveGame();
    }

    public void CreateNewSave(string selectedCharacterID)
    {
        CurrentSave = new SaveData();
        CurrentSave.totalPlayTime = 0;
        CurrentSave.player = new PlayerData();
        CurrentSave.player.characterID = selectedCharacterID;
        CurrentSave.currentScene = "Misty Forest";
        CurrentSave.totalPlayTime = 0f;

        // Ottengo lo slot selezionato
        int slotIndex = LoadFileSlotManager.Instance.GetSelectedSlotIndex();
        if (slotIndex < 0)
        {
            Debug.LogError("Nessun file selezionato!");
            return;
        }

        // Nome file basato sull'indice
        string fileName = $"{slotIndex}.json";

        CurrentSavePath = Path.Combine(_savesFolder, fileName);

        // Segna lo slot come usato
        LoadFileSlotManager.Instance.MarkSlotUsed(slotIndex);

        SaveGame();
    }

    public void SaveGame()
    {
        if (CurrentSave == null || string.IsNullOrEmpty(CurrentSavePath))
        {
            Debug.LogWarning("Nessun salvataggio attivo!");
            return;
        }

        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>(true)
                  .OfType<ISaveable>()
                  .ToArray();

        foreach (ISaveable s in saveables)
        {
            s.SaveState(CurrentSave);
        }

        CurrentSave.lastSaveDate = DateTime.Now.ToString("yyyy/MM/dd");

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName != "Menu")
            CurrentSave.currentScene = sceneName;

        if (SceneTime.Instance != null)
            CurrentSave.totalPlayTime = SceneTime.Instance.TimePlay;
        else
            CurrentSave.totalPlayTime = 0f;

        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(CurrentSavePath, json);

        Debug.Log($"Salvataggio completato: {CurrentSavePath}");
    }

    public void LoadGame(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Il file di salvataggio non esiste!");
            return;
        }

        string json = File.ReadAllText(path);
        CurrentSave = JsonUtility.FromJson<SaveData>(json);
        CurrentSavePath = path;

        Debug.Log("CURRENT SAVE " + CurrentSave);

        ISaveable[] saveables = FindObjectsOfType<MonoBehaviour>(true)
                  .OfType<ISaveable>()
                  .ToArray();

        foreach (ISaveable s in saveables)
        {
            s.LoadState(CurrentSave);
        }

        Debug.Log($"Caricato salvataggio da: {path}");
    }

    public void LoadGame()
    {
        if (CurrentSavePath == null) return;

        LoadGame(CurrentSavePath);
    }

    public void DeleteGame(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Il file di salvataggio non esiste!");
            return;
        }

        File.Delete(path);
        Debug.Log("File eliminato!");
    }

    public List<string> GetAllSaveFiles()
    {
        return new List<string>(Directory.GetFiles(_savesFolder, "*.json"));
    }

    public void SetCurrentPath(GameObject parent)
    {
        string saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        LoadFileSlotManager.Instance.SetSelectedFile(parent);
        int slotIndex = LoadFileSlotManager.Instance.GetSelectedSlotIndex();

        string fileName = $"{slotIndex}.json";

        CurrentSavePath = Path.Combine(saveFolder, fileName);
    }

    public SaveData GetSaveByPath()
    {
        string json = File.ReadAllText(CurrentSavePath);
        return CurrentSave = JsonUtility.FromJson<SaveData>(json);
    }
}