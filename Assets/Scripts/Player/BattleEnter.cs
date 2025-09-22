using UnityEngine;

public class BattleEnter : MonoBehaviour
{
    [SerializeField] private GameObject _battleScene;
    [SerializeField] private GameObject _player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.RaiseBattleEnter(_battleScene, _player);
            Destroy(gameObject);
        }
    }
}
