using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    void Start()
    {
        SaveData save = SaveSystem.Instance.CurrentSave;
        MonoBehaviour[] saveables = FindObjectsOfType<MonoBehaviour>(true);

        foreach (MonoBehaviour s in saveables)
        {
            if (s is ISaveable saveable)
            {
                saveable.LoadState(save);
            }
        }
    }
}
