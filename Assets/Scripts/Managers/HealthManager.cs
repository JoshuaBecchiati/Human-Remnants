using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _staminaBar;
    [SerializeField] private Player _player;

    private void Awake()
    {
        _player.OnChangeHealth += ChangeHealthHandler;
        _player.OnChangeStamina += ChangeStaminaHandler;
        _player.OnRefillStamina += RefillStamina;
    }

    private void OnDestroy()
    {
        _player.OnChangeHealth -= ChangeHealthHandler;
        _player.OnChangeStamina -= ChangeStaminaHandler;
        _player.OnRefillStamina -= RefillStamina;
    }

    private void ChangeHealthHandler(float health)
    {
            _healthBar.fillAmount = health / 100f;
    }

    private void ChangeStaminaHandler(float stamina)
    {
        _staminaBar.fillAmount = stamina / 100f;
    }

    private void RefillStamina(float regainStamina)
    {
        _staminaBar.fillAmount = regainStamina / 100f;
    }
}
