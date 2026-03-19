using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarView : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Unit _unit;

    public void Init(Unit unit)
    {
        _unit = unit;
        
        _unit.Health.OnTakeDamage += OnHealthChanged;
        _unit.Health.OnHeal += OnHealthChanged;
    }

    private void OnHealthChanged(float _)
    {
        _healthBar.fillAmount = _unit.Health.CurrentHealth / _unit.Health.MaxHealth;
    }

    private void OnDestroy()
    {
        _unit.Health.OnTakeDamage -= OnHealthChanged;
        _unit.Health.OnHeal -= OnHealthChanged;
    }
}
