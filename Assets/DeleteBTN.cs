using UnityEngine;

public class DeleteBTN : MonoBehaviour
{
    public void OnButtonPressed(GameObject parent)
    {
        LoadFileSlotManager.Instance.SetSelectedFile(parent);
        LoadFileSlotManager.Instance.DestroyFile();

        parent.transform.Find("Save File BTN").gameObject.SetActive(false);
        parent.transform.Find("New File BTN").gameObject.SetActive(true);
    }
}
