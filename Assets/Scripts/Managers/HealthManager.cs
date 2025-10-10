using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Player _player;

    private void Awake()
    {
        _player.OnChangeHealth += ChangeHealthHandler;
    }

    private void OnDestroy()
    {
        _player.OnChangeHealth -= ChangeHealthHandler;
    }

    private void ChangeHealthHandler(float health)
    {
        _healthBar.fillAmount = health / 100f;
    }
}
