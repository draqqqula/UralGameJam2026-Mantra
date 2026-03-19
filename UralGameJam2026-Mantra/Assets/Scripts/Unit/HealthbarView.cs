using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarView : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Unit _unit;

    [SerializeField] private float _textDuration;
    [SerializeField] private TextMeshProUGUI _healthChangingText;
    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _healColor;
    
    private Coroutine _textCoroutine;

    public void Init(Unit unit)
    {
        _unit = unit;
        
        _unit.Health.OnTakeDamage += OnDamaged;
        _unit.Health.OnHeal += OnHeal;
    }

    private void OnDamaged(float value)
    {
        UpdateText("- " + value.ToString(), _damageColor);
        OnHealthChanged();
    }

    private void OnHeal(float value)
    {
        UpdateText("+ " + value.ToString(), _healColor);
        OnHealthChanged();
    }
    
    private void OnHealthChanged()
    {
        _healthBar.fillAmount = _unit.Health.CurrentHealth / _unit.Health.MaxHealth;
    }
    
    private void UpdateText(string text, Color color)
    {
        _healthChangingText.gameObject.SetActive(true);
        _healthChangingText.text = text;
        _healthChangingText.color = color;
        
        if (_textCoroutine != null) StopCoroutine(_textCoroutine);
        _textCoroutine = StartCoroutine(TextCoroutine());
    }

    private IEnumerator TextCoroutine()
    {
        yield return new WaitForSeconds(_textDuration);
        _healthChangingText.gameObject.SetActive(false);
        _textCoroutine = null;
    }

    private void OnDestroy()
    {
        _unit.Health.OnTakeDamage -= OnDamaged;
        _unit.Health.OnHeal -= OnHeal;
    }
}
