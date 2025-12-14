using UnityEngine;

public class SaveableObject : MonoBehaviour, ISaveable
{
    public string uniqueID;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(uniqueID))
        {
            uniqueID = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif

    public string GetID()
    {
        return uniqueID;
    }


    public virtual void LoadState(SaveData save)
    {
        // implementato negli oggetti specifici
    }

    public virtual void SaveState(SaveData save)
    {
        // implementato negli oggetti specifici
    }
}