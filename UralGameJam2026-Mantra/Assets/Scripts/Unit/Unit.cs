using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using R3;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    public Transform RenderCameraPoint;
    public bool IsAlive => Health.CurrentHealth > 0;
    public string UnitName;
    public int Variant;
    public bool ShouldCreateAura = true;

    [field: SerializeField] public UnitType UnitType { get; private set; }
    [field: SerializeField] public bool IsMainHero { get; set; }

    public UnitHealth Health;
    public UnitDamage Damage;
    public UnitTurn UnitTurn { get; private set; }


    public List<UnitAction> UnitActions = new();

    [SerializeField] private bool _isUniqueUnit = false;
    private float _currentUltimateCooldown, _maxUltimateCooldown;

    [SerializeField] private Transform _healthbarPoint;
    [SerializeField] private Transform _haloPoint;
    [SerializeField] private Transform _auraPoint;
    [SerializeField] private Transform _ultimatePoint;

    [SerializeField] private HealthbarView _healthbarPrefab;
    [SerializeField] private TurnAuraView _auraPrefab;
    [SerializeField] private UnitHaloView _haloPrefab;
    [SerializeField] private UnitUltimateCooldownView _ultimatePrefab;

    private Transform _healthBarTransform, _auraTransform, _haloTransform, _ultimateTransform;
    private TurnManager _turnManager;

    public event Action OnDestroyed;
    public event Action<UnitAttachedSkill> OnAttachedSkillAdded;
    public event Action<UnitAttachedSkill> OnAttachedSkillRemoved;
    
    public List<UnitAttachedSkill> AttachedSkills = new();
    [SerializeField] private UnitModifiersViewController _modifiersViewController;
        

    private void Start()
    {
        var canvas = ServiceLocator.Instance.GetService<UnitCanvas>();
        _turnManager = ServiceLocator.Instance.GetService<TurnManager>();

        UnitTurn = GetComponent<UnitTurn>();

        //if (ShouldCreateAura)
        //{
        //    InstantiateAura();
        //}

        var halo = Instantiate(_haloPrefab, canvas.transform);
        halo.transform.position = _haloPoint.position;
        halo.Init(_haloPoint);
        _haloTransform = halo.transform;

        var ultimate = Instantiate(_ultimatePrefab, canvas.transform);
        ultimate.transform.position = _ultimatePoint.position;
        _ultimateTransform = ultimate.transform;

        var healthbar =  Instantiate(_healthbarPrefab, canvas.transform);
        healthbar.transform.position = _healthbarPoint.position;
        healthbar.Init(this);

        _healthBarTransform = healthbar.transform; 
    }

    public void ClearModifiers()
    {
        Health.CurrentDefense.ClearModifiers();
        Damage.MinDamage.ClearModifiers();
        Damage.MaxDamage.ClearModifiers();
        Damage.CritMultiplyer.ClearModifiers();
        Damage.CritChance.ClearModifiers();

        foreach (var skill in AttachedSkills)
        {
            OnAttachedSkillRemoved?.Invoke(skill);
        }
        
        AttachedSkills.Clear();
    }

    public bool RespondSkill(Unit enemy)
    {
        foreach (var skill in AttachedSkills)
        {
            var successRespond = skill.TryRespond(enemy);
            if (successRespond) return true;
        }

        return false;
    }

    public void AttachSkill(UnitAttachedSkill skill)
    {
        var attachedBefore = AttachedSkills.FirstOrDefault(x => x.Equals(skill));
        if (attachedBefore != null) return;

        AttachedSkills.Add(skill);
        OnAttachedSkillAdded?.Invoke(skill);
    }

    public void CheckAttached()
    {
        foreach(var attached in AttachedSkills.ToList())
        {
            if (!attached.UpdateTurns())
            {
                AttachedSkills.Remove(attached);
                OnAttachedSkillRemoved?.Invoke(attached);
            }
        }
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

        if (!_ultimateTransform) return;
        _ultimateTransform.position = _ultimatePoint.position;

        if (!_haloTransform) return;
        _haloTransform.position = _haloPoint.position;

        if (!_auraTransform) return;
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
    
    public void HideHalo()
    {
        _haloTransform.gameObject.SetActive(false);
    }

    public void ShowHalo()
    {
        _haloTransform.gameObject.SetActive(true);
    }

    public void HideUltimate()
    {
        _haloTransform.gameObject.SetActive(false);
    }

    public void ShowUltimate()
    {
        _haloTransform.gameObject.SetActive(true);
    }


    public void Init()
    {
        Health.Setup();
        Damage.Setup();

        Health.OnDeath += ClearModifiers;
        _modifiersViewController.Init(this);
    }
    
    public async UniTask Use<T>(Unit target) where T : UnitAction
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(T));
        if (action == null) return;

        _turnManager.AddTurn(this, action);

        action.Plan(this, target);

        var ultimate = UnitActions.FirstOrDefault(x => x.GetType() == typeof(UltimateAttackAction)) as UltimateAttackAction;
        ultimate?.IncreaseCooldown(out _currentUltimateCooldown, out _maxUltimateCooldown, action.CooldownStep);

        _haloTransform.GetComponent<UnitHaloView>().SetHalo(_currentUltimateCooldown, _maxUltimateCooldown);
        _ultimateTransform.GetComponent<UnitUltimateCooldownView>().UpdateView(_currentUltimateCooldown, _maxUltimateCooldown);

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
        ultimate.IncreaseCooldown(out _currentUltimateCooldown, out _maxUltimateCooldown);

        _haloTransform.GetComponent<UnitHaloView>().SetHalo(_currentUltimateCooldown, _maxUltimateCooldown);
        _ultimateTransform.GetComponent<UnitUltimateCooldownView>().UpdateView(_currentUltimateCooldown, _maxUltimateCooldown);

        if (ultimate.CanUse())
        {
            ultimate.Plan(this, target);
            _ultimateTransform.GetComponent<UnitUltimateCooldownView>()?.Hide();

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
            Type = UnitType,
            Variant = Variant,
            
            MaxHealth = Health.MaxHealth,
            MaxDefaultHealth = Health.MaxDefaultHealth,

            Defense = Health.CurrentDefense.ModValue,
            MaxDefense = Health.MaxDefense,
            MaxDefaultDefense = Health.MaxDefaultDefense,

            ModifierEffectsMinDamage = new(Damage.MinDamage.Modifiers),
            ModifierEffectsMaxDamage = new(Damage.MaxDamage.Modifiers),

            ModifierEffectsCritChance = new(Damage.CritChance.Modifiers),
            ModifierEffectsCritMultiplyer = new(Damage.CritMultiplyer.Modifiers),
            ModifierEffectsDefense = new(Health.CurrentDefense.Modifiers),

            MinDefaultDamage = Damage.MinDefaultDamage,
            MaxDefaultDamage = Damage.MaxDefaultDamage,

            DefaultCritChance = Damage.DefaultCritChance,
            DefaultCritMultiplyer = Damage.DefaultCritMultiplyer
        };

        return SerializeUnit;
    }

    public void Deserialize(SerializeUnit serializeUnit)
    {
        SetName(serializeUnit.Name);
        Variant = serializeUnit.Variant;
        Health.CurrentHealth = serializeUnit.MaxHealth;
        Health.MaxHealth = serializeUnit.MaxHealth;
        Health.MaxDefaultHealth = serializeUnit.MaxDefaultHealth;

        Health.CurrentDefense = new(serializeUnit.Defense);
        Health.MaxDefense = serializeUnit.MaxDefense;
        Health.MaxDefaultDefense = serializeUnit.MaxDefaultDefense;
        
        Damage.MinDefaultDamage = serializeUnit.MinDefaultDamage;
        Damage.MaxDefaultDamage = serializeUnit.MaxDefaultDamage;

        Damage.DefaultCritChance = serializeUnit.DefaultCritChance;
        Damage.DefaultCritMultiplyer = serializeUnit.DefaultCritMultiplyer;
        
        Damage.Setup();

        Damage.MinDamage.Modifiers = new(serializeUnit.ModifierEffectsMinDamage);
        Damage.MaxDamage.Modifiers = new(serializeUnit.ModifierEffectsMaxDamage);

        Damage.CritChance.Modifiers = new(serializeUnit.ModifierEffectsCritChance);
        Damage.CritMultiplyer.Modifiers = new(serializeUnit.ModifierEffectsCritMultiplyer);
    }

    public void Resurrect()
    {
        Health.ApplyHealToMax(); 
        GetComponent<UnitAnimator>()?.Play(UnitAnimation.Idle, out _);
        GetComponent<UnitRetired>()?.Resurrect();
    }

    public void Resurrect(float heal)
    {
        Health.ApplyHeal(heal);
        GetComponent<UnitAnimator>()?.Play(UnitAnimation.Idle, out _);
        GetComponent<UnitRetired>()?.Resurrect();
    }

    [ContextMenu("KillUnit")]
    private void KillUnit()
    {
        Health.ApplyFatalDamage();
    }

    public void OnDestroy()
    {
        if (_auraTransform)
        {
            Destroy(_auraTransform.gameObject);
        }
        if (_haloTransform)
        {
            Destroy(_haloTransform.gameObject);
        }
        if (_ultimateTransform)
        {
            Destroy(_ultimateTransform.gameObject);
        }
        OnDestroyed?.Invoke();
        Health.OnDeath -= ClearModifiers;
        _modifiersViewController.Disable(this);
    }
}

public enum UnitType {Warrior, Ranger, Spearman, Mage}
