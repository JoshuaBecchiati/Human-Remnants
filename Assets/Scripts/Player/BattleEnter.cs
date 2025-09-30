using UnityEngine;

public class BattleEnter : MonoBehaviour
{
    [SerializeField] private BattleSettings _battleSettings;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.BattleStart(_battleSettings);
        }
    }
}
