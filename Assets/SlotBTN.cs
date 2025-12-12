using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlotBTN : MonoBehaviour
{
    public void OnButtonPressed(GameObject parent)
    {
        LoadFileSlotManager.Instance.SetSelectedFile(parent);
    }

    public void OnLoadGame(GameObject parent)
    {
        Time.timeScale = 1f;

        SaveSystem.Instance.SetCurrentPath(parent);
        SaveData sd = SaveSystem.Instance.GetSaveByPath();
        int index = SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{sd.currentScene}.unity");

        //Debug.Log($"Index scene {index} Current scene Assets/Scenes/{sd.currentScene}");

        MenuManager.Instance.ChangeScene(index);
    }
}
