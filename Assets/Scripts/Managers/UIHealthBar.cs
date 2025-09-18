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
        UpdateBar(unit.Health, unit.MaxHealth);

        // Subscrive l’evento di UnitBase
        unit.OnUnitTookDamage += UpdateBar;

        if (_nameText != null)
            _nameText.text = unit.Name;
    }

    private void UpdateBar(float current, float max)
    {
        _image.fillAmount = current / max;
    }
}
