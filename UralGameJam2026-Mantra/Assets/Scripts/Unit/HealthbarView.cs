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
    
    [SerializeField] private TextMeshProUGUI _healthTextPrefab;
    [SerializeField] private SpawnerInSquare _spawnerInSquare;
    
    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _healColor;
    
    public void Init(Unit unit)
    {
        _unit = unit;
        
        _unit.Health.OnTakeDamage += OnDamaged;
        _unit.Health.OnHeal += OnHeal;
        _unit.OnDestroyed += Destroy;
    }

    private void OnDamaged(float value)
    {
        SpawnText("- " + value.ToString(), _damageColor);
        OnHealthChanged();
    }

    private void OnHeal(float value)
    {
        SpawnText("+ " + value.ToString(), _healColor);
        OnHealthChanged();
    }
    
    private void OnHealthChanged()
    {
        _healthBar.fillAmount = _unit.Health.CurrentHealth / _unit.Health.MaxHealth;
    }
    
    private void SpawnText(string text, Color color)
    {
        var healthText = _spawnerInSquare.Spawn(_healthTextPrefab);
        
        healthText.gameObject.SetActive(true);
        healthText.text = text;
        healthText.color = color;
        
        StartCoroutine(TextCoroutine(healthText));
    }

    private IEnumerator TextCoroutine(TextMeshProUGUI healthText)
    {
        yield return new WaitForSeconds(_textDuration);
        Destroy(healthText.gameObject);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _unit.Health.OnTakeDamage -= OnDamaged;
        _unit.Health.OnHeal -= OnHeal;
        _unit.OnDestroyed -= Destroy;
    }
}