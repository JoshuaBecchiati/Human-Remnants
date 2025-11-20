using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _nameText;

    private UnitBase _unit;

    private void OnDisable()
    {
        if (_unit != null)
            _unit.OnUnitTookDamage -= UpdateBar;
    }

    public void Setup(UnitBase unit)
    {
        _unit = unit;

        // Inizializza subito la barra
        UpdateBar(_unit.Health, _unit.MaxHealth);

        // Subscrive l’evento di UnitBase
        _unit.OnUnitTookDamage += UpdateBar;
        _unit.OnHeal += UpdateBar;

        if (_nameText != null)
            _nameText.text = _unit.Name;
    }

    private void UpdateBar(float current, float max)
    {
        _image.fillAmount = current / max;
    }
}
