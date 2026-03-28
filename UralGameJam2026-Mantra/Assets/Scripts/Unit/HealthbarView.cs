using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarView : MonoBehaviour
{
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _healthBarDelayImage;
    [SerializeField] private Unit _unit;

    [SerializeField] private float _textDuration;
    
    [SerializeField] private TextMeshProUGUI _healthTextPrefab;
    [SerializeField] private SpawnerInSquare _spawnerInSquare;
    
    [SerializeField] private TextMeshProUGUI _healthProgressText;
    
    [SerializeField] private Color _damageColor;
    [SerializeField] private Color _healColor;

    [SerializeField] private AnimationCurve _fadeCurve;
    
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _delayDuration;
    private Sequence _healthBarSequence;
    
    public void Init(Unit unit)
    {
        _unit = unit;
        
        _unit.Health.OnTakeDamage += OnDamaged;
        _unit.Health.OnHeal += OnHeal;
        _unit.OnDestroyed += Destroy;
        
        _healthProgressText.text = _unit.Health.CurrentHealth + " / " + _unit.Health.MaxHealth;
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
        _healthBarSequence?.Kill();
        
        _healthBarSequence = DOTween.Sequence();
        _healthBarSequence.Insert(_delayDuration, _healthBarDelayImage
            .DOFillAmount(_unit.Health.CurrentHealth / _unit.Health.MaxHealth, _animationDuration)
            .SetLink(gameObject));
        _healthBar.fillAmount = _unit.Health.CurrentHealth / _unit.Health.MaxHealth;
        
        _healthProgressText.text = Mathf.Clamp(_unit.Health.CurrentHealth, 0, _unit.Health.MaxHealth) + " / " + _unit.Health.MaxHealth;
    }
    
    private void SpawnText(string text, Color color)
    {
        var healthText = _spawnerInSquare.Spawn(_healthTextPrefab);
        
        healthText.gameObject.SetActive(true);
        healthText.text = text;
        healthText.color = color;
        
        healthText.transform
            .DOLocalMoveY(healthText.transform.localPosition.y + 2, _textDuration)
            .SetLink(healthText.gameObject);

        healthText
            .DOFade(0f, _textDuration)
            .SetEase(_fadeCurve)
            .SetLink(healthText.gameObject)
            .OnComplete(() => Destroy(healthText.gameObject));
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