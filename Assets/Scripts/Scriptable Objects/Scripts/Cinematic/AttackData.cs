using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "NewAttack", menuName = "Cinematic/Combat/Attack")]
public class AttackData : ScriptableObject
{
    public string attackName;           // Nome dell'attacco
    public GameObject attackPrefab;     // Prefab della timeline
}
