using UnityEngine;

[CreateAssetMenu(fileName = "Battle settings", menuName ="Battle/Battle settings")]

public class BattleSettings : ScriptableObject
{
    public GameObject[] enemies;
    public ItemData[] drops;
    // public GameObject _scene;
}
