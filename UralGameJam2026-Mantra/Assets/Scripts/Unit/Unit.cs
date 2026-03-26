using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform RenderCameraPoint;
    public bool IsAlive => Health.CurrentHealth > 0;
    public string UnitName;
    public bool ShouldCreateAura = true;

    public UnitHealth Health;
    public UnitDamage Damage;
    public UnitTurn UnitTurn { get; private set; }


    public List<UnitAction> UnitActions = new();

    [SerializeField] private bool _isUniqueUnit = false;
    [SerializeField] private int _attackCooldown;

    [SerializeField] private Transform _healthbarPoint;
    [SerializeField] private Transform _auraPoint;
    [SerializeField] private HealthbarView _healthbarPrefab;
    [SerializeField] private TurnAuraView _auraPrefab;

    private Transform _healthBarTransform, _auraTransform;
    private TurnManager _turnManager;

    public event Action OnDestroyed; 
        

    private void Start()
    {
        var canvas = ServiceLocator.Instance.GetService<UnitCanvas>();
        _turnManager = ServiceLocator.Instance.GetService<TurnManager>();

        UnitTurn = GetComponent<UnitTurn>();

        if (ShouldCreateAura)
        {
            InstantiateAura();
        }

        var healthbar =  Instantiate(_healthbarPrefab, canvas.transform);
        healthbar.transform.position = _healthbarPoint.position;
        healthbar.Init(this);

        _healthBarTransform = healthbar.transform; 
    }
    
    public void InstantiateAura()
    {
        if (_auraTransform) return;
 
        var canvas = ServiceLocator.Instance.GetService<UnitCanvas>();

        var aura = Instantiate(_auraPrefab, canvas.transform);
        aura.transform.position = _auraPoint.position;
        aura.Init(this, UnitTurn);

        _auraTransform = aura.transform;
    }

    public void UpdateRenderCameraPoint()
    {
        if(transform.eulerAngles.y == 180)
        {
            var point = RenderCameraPoint.transform.position;
            point.z = -point.z;

            RenderCameraPoint.transform.position = point;
        }
    }

    public void UpdateUIPosition()
    {
        if (!_healthBarTransform) return;
        _healthBarTransform.position = _healthbarPoint.position;

        if(!_auraTransform) return;
        _auraTransform.position = _auraPoint.position;
    }

    public void HideAura()
    {
        if (!_auraTransform) return;
        _auraTransform.gameObject.SetActive(false);
    }

    public void ShowAura()
    {
        if (!_auraTransform) return;
        _auraTransform.gameObject.SetActive(true);
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

    public async UniTask Use<T>(Unit target) where T : UnitAction
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(T));
        if (action == null) return;

        _turnManager.AddTurn(this, action);

        action.Plan(this, target);
        await action.Execute();
    }

    public UnitAction Get<T>() where T : UnitAction
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(T));
        if (action == null) return null;

        return action;
    }

    public async UniTask UpdateUltimateCooldown(Unit target)
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(UltimateAttackAction));
        if (action == null) return;

        var ultimate = action as UltimateAttackAction;
        ultimate.DecreaseCooldown();

        if (ultimate.CanUse())
        {
            ultimate.Plan(this, target);
            await ultimate.Execute();
        }
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
        if (_auraTransform)
        {
            Destroy(_auraTransform.gameObject);
        }
        OnDestroyed?.Invoke();
    }
}
