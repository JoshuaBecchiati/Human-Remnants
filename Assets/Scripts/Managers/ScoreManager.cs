using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmpScore;
    [SerializeField] private int _totalScore;

    // Start is called before the first frame update
    void Start()
    {
        // Trova tutti i nemici già in scena e si iscrive
        foreach (var enemy in FindObjectsOfType<MonoBehaviour>())
        {
            if (enemy is IEnemy e)
            {
                e.OnDeath += HandleEnemyDeath;
            }
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in FindObjectsOfType<MonoBehaviour>())
        {
            if (enemy is IEnemy e)
            {
                e.OnDeath -= HandleEnemyDeath;
            }
        }
    }

    private void HandleEnemyDeath(IEnemy enemy)
    {
        _totalScore += enemy.Score;
        _tmpScore.text = $"Score - {_totalScore}";
    }
}
