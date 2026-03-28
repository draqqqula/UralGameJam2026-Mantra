using System.Collections.Generic;

[System.Serializable]
public class SerializeUnit
{
    public string Name;
    public UnitType Type;
    public int Variant;
    
    public float MaxHealth;
    public float MaxDefaultHealth;

    public float Defense;
    public float MaxDefense;
    public float MaxDefaultDefense;

    public int UltimateAttackCooldown;

    public List<ModifierEffect> ModifierEffectsMinDamage = new();
    public List<ModifierEffect> ModifierEffectsMaxDamage = new();

    public List<ModifierEffect> ModifierEffectsCritChance = new();
    public List<ModifierEffect> ModifierEffectsCritMultiplyer = new();
    public List<ModifierEffect> ModifierEffectsDefense = new();

    public float MinDefaultDamage;
    public float MaxDefaultDamage;

    public float DefaultCritChance;
    public float DefaultCritMultiplyer;
}
