using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadFileSlotManager : MonoBehaviour
{
    [SerializeField] private List<FileData> m_files;
    [SerializeField] private List<CharacterPortraitEntry> m_portraits;

    private int _selectedSlotIndex = -1;

    public static LoadFileSlotManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void BindUISlots(List<GameObject> uiFiles)
    {
        for (int i = 0; i < m_files.Count; i++)
        {
            m_files[i].SetFile(uiFiles[i]);
        }

        LoadSaveFiles();
    }

    public void SetSelectedFile(GameObject selectedFile)
    {
        for (int i = 0; i < m_files.Count; i++)
        {
            if (m_files[i].File == selectedFile)
            {
                _selectedSlotIndex = i;
                break;
            }
        }
    }

    public int GetSelectedSlotIndex()
    {
        return _selectedSlotIndex;
    }

    public void MarkSlotUsed(int index)
    {
        m_files[index].SetUsed(true);
    }

    public void LoadSaveFiles()
    {
        string saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        for (int i = 0; i < m_files.Count; i++)
        {
            string expectedFile = Path.Combine(saveFolder, $"{i}.json");

            // UI di riferimento
            GameObject uiFile = m_files[i].File;

            if (!File.Exists(expectedFile))
            {
                // Non esiste nessun salvataggio → mostra "New File BTN"
                uiFile.transform.Find("New File BTN").gameObject.SetActive(true);
                uiFile.transform.Find("Save File BTN").gameObject.SetActive(false);
                continue;
            }

            // Esiste il file → slot usato
            m_files[i].SetUsed(true);

            // Aggiorna UI
            uiFile.transform.Find("New File BTN").gameObject.SetActive(false);
            GameObject saveBtn = uiFile.transform.Find("Save File BTN").gameObject;
            saveBtn.SetActive(true);

            // Carica dati
            string json = File.ReadAllText(expectedFile);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // --- Play Time ---
            int minutes = (int)(data.totalPlayTime / 60f);
            int hours = minutes / 60;
            minutes %= 60;

            string playTimeString = $"{hours:D2}:{minutes:D2}";

            TextMeshProUGUI playtime = saveBtn.transform.Find("Front/TMP_Play_Time").GetComponent<TextMeshProUGUI>();

            playtime.text = $"Play time {playTimeString}";

            playtime = saveBtn.transform.Find("Front (1)/TMP_Play_Time").GetComponent<TextMeshProUGUI>();

            playtime.text = $"Play time {playTimeString}";

            // --- Portrait ---
            Image portrait = saveBtn.transform.Find("Front/Image_Protagonist").GetComponent<Image>();

            Sprite sp = m_portraits.Find(c => c.type.ToString() == data.player.characterID).portrait;

            portrait.sprite = sp;

            portrait = saveBtn.transform.Find("Front (1)/Image_Protagonist").GetComponent<Image>();

            portrait.sprite = sp;

            // --- Last save date ---
            TextMeshProUGUI lsd = saveBtn.transform.Find("Front/TMP_Date_Last_save").GetComponent<TextMeshProUGUI>();

            lsd.text = data.lastSaveDate;

            lsd = saveBtn.transform.Find("Front (1)/TMP_Date_Last_save").GetComponent<TextMeshProUGUI>();

            lsd.text = data.lastSaveDate;

            // --- Zone name ---
            TextMeshProUGUI zn = saveBtn.transform.Find("Front/TMP_Zone_Name").GetComponent<TextMeshProUGUI>();

            zn.text = data.currentScene;

            zn = saveBtn.transform.Find("Front (1)/TMP_Zone_Name").GetComponent<TextMeshProUGUI>();

            zn.text = data.currentScene;
        }
    }
}
