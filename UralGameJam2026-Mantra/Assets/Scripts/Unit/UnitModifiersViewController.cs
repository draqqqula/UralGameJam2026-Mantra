using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitModifiersViewController : MonoBehaviour
{
    [SerializeField] private ModifierViewData[] _modifierViewDatas;
    
    public void Init(Unit unit)
    {
        foreach (var modifierViewData in _modifierViewDatas)
        {
            modifierViewData.EffectView.SetActive(false);
        }
        
        unit.Health.CurrentDefense.OnModifierAdded += OnModifierAdded;
        unit.Damage.MinDamage.OnModifierAdded += OnModifierAdded;
        unit.Damage.MaxDamage.OnModifierAdded += OnModifierAdded;
        unit.Damage.CritMultiplyer.OnModifierAdded += OnModifierAdded;
        unit.Damage.CritChance.OnModifierAdded += OnModifierAdded;
        
        unit.Health.CurrentDefense.OnModifierRemoved += OnModifierRemoved;
        unit.Damage.MinDamage.OnModifierRemoved += OnModifierRemoved;
        unit.Damage.MaxDamage.OnModifierRemoved += OnModifierRemoved;
        unit.Damage.CritMultiplyer.OnModifierRemoved += OnModifierRemoved;
        unit.Damage.CritChance.OnModifierRemoved += OnModifierRemoved;

        unit.OnAttachedSkillAdded += OnModifierAdded;
        unit.OnAttachedSkillRemoved += OnModifierRemoved;
    }

    private void OnModifierAdded(ModifierEffect modifierEffect)
    {
        var viewData = _modifierViewDatas.FirstOrDefault(m => m.Name == modifierEffect.Name);
        if (!viewData.EffectView.activeInHierarchy)
        {
            viewData.EffectView.SetActive(true);
        }
    }

    private void OnModifierRemoved(ModifierEffect modifierEffect)
    {
        var viewData = _modifierViewDatas.FirstOrDefault(m => m.Name == modifierEffect.Name);
        if (viewData.EffectView.activeInHierarchy)
        {
            viewData.EffectView.SetActive(false);
        }
    }

    public void Disable(Unit unit)
    {
        unit.Health.CurrentDefense.OnModifierAdded -= OnModifierAdded;
        unit.Damage.MinDamage.OnModifierAdded -= OnModifierAdded;
        unit.Damage.MaxDamage.OnModifierAdded -= OnModifierAdded;
        unit.Damage.CritMultiplyer.OnModifierAdded -= OnModifierAdded;
        unit.Damage.CritChance.OnModifierAdded -= OnModifierAdded;
        
        unit.Health.CurrentDefense.OnModifierRemoved -= OnModifierRemoved;
        unit.Damage.MinDamage.OnModifierRemoved -= OnModifierRemoved;
        unit.Damage.MaxDamage.OnModifierRemoved -= OnModifierRemoved;
        unit.Damage.CritMultiplyer.OnModifierRemoved -= OnModifierRemoved;
        unit.Damage.CritChance.OnModifierRemoved -= OnModifierRemoved;
    }
}

[Serializable]
public class ModifierViewData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public GameObject EffectView { get; private set; }
}