using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool IsAlive => Health.CurrentHealth > 0;
    public string UnitName;

    public UnitHealth Health;
    public UnitDamage Damage;

    public List<UnitAction> UnitActions = new();

    [SerializeField] private bool _isUniqueUnit = false;
    [SerializeField] private int _attackCooldown;

    [SerializeField] private Transform _healthbarPoint;
    [SerializeField] private HealthbarView _healthbarPrefab;

    private Transform _healthBarTransform;
    private TurnManager _turnManager;

    public event Action OnDestroyed; 
        
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        var canvas = ServiceLocator.Instance.GetService<UnitCanvas>();
        _turnManager = ServiceLocator.Instance.GetService<TurnManager>();
        
        var healthbar =  Instantiate(_healthbarPrefab, canvas.transform);
        healthbar.transform.position = _healthbarPoint.position;
        healthbar.Init(this);

        _healthBarTransform = healthbar.transform; 
    }

    public void UpdateHealthbarPosition()
    {
        if (!_healthBarTransform) return;
        _healthBarTransform.position = _healthbarPoint.position;
    }

    public void HideHealthbars()
    {
        _healthBarTransform.gameObject.SetActive(false);
    }

    public void ShowHealthbars()
    {
        _healthBarTransform.gameObject.SetActive(true);
    }

    public void Init()
    {
        Health.Setup();
        Damage.Setup();
    }

    public void Use<T>(Unit target) where T : UnitAction
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(T));
        if (action == null) return;

        _turnManager.AddTurn(this, action);

        action.Plan(this, target);
    }

    public UnitAction Get<T>() where T : UnitAction
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(T));
        if (action == null) return null;

        return action;
    }

    public void UpdateUltimateCooldown()
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(UltimateAttackAction));
        if (action == null) return;

        var ultimate = action as UltimateAttackAction;
        ultimate.DecreaseCooldown();

        _turnManager.AddTurn(this, ultimate);
    }

    public void SetName(string name)
    {
        if (_isUniqueUnit) return;

        UnitName = name;
    }

    public SerializeUnit Serialize()
    {
        var SerializeUnit = new SerializeUnit
        {
            Name = UnitName,

            Health = Health.CurrentHealth,
            MaxHealth = Health.MaxHealth,
            MaxDefaultHealth = Health.MaxDefaultHealth,

            Defense = Health.CurrentDefense,
            MaxDefense = Health.MaxDefense,
            MaxDefaultDefense = Health.MaxDefaultDefense,

            UltimateAttackCooldown = _attackCooldown,

            ModifierEffectsMinDamage = new(Damage.MinDamage.Modifiers),
            ModifierEffectsMaxDamage = new(Damage.MaxDamage.Modifiers),

            ModifierEffectsCritChance = new(Damage.CritChance.Modifiers),
            ModifierEffectsCritMultiplyer = new(Damage.CritMultiplyer.Modifiers),

            MinDefaultDamage = Damage.MinDefaultDamage,
            MaxDefaultDamage = Damage.MaxDefaultDamage,

            DefaultCritChance = Damage.DefaultCritChance,
            DefaultCritMultiplyer = Damage.DefaultCritMultiplyer
        };

        return SerializeUnit;
    }

    public void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }
}
