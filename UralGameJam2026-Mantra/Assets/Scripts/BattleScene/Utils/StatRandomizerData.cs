using UnityEngine;

[CreateAssetMenu(fileName = "StatsData_", menuName = "Stats")]
public class StatRandomizerData : ScriptableObject
{
    public UnitType UnitType;
    public int VariantCount = 3;
    public float MinMinDamage, MaxMinDamage;
    public float MinMaxDamage, MaxMaxDamage;
    public float MinHealth, MaxHealth;
    public float MinCritChance, MaxCritChance;
    public float MinCritMulti, MaxCritMulti;
    public float MinDefense, MaxDefense;
}
