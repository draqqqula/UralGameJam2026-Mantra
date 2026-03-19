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


    private void Awake()
    {
        Init();
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

        TestBattleManager.Instance.AddTurn(this, action);

        action.Plan(this, target);
    }

    public void UpdateUltimateCooldown()
    {
        var action = UnitActions.FirstOrDefault(x => x.GetType() == typeof(UltimateAttackAction));
        if (action == null) return;

        var ultimate = action as UltimateAttackAction;
        ultimate.DecreaseCooldown();

        TestBattleManager.Instance.AddTurn(this, ultimate);

        TestBattleManager.Instance.UpdateOrder();
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
}
