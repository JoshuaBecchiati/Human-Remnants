public interface ISaveable
{
    string GetID();
    void LoadState(SaveData save);
    void SaveState(SaveData save);
}