using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitDamage
{
    public ModifableValue MinDamage { get; set; }
    public ModifableValue MaxDamage { get; set; }
    [Range(0f, 1f)]
    public ModifableValue CritChance { get; set; }
    public ModifableValue CritMultiplyer { get; set; }

    [SerializeField] private float _minDefaultDamage;
    [SerializeField] private float _maxDefaultDamage;
    [Space(10)]

    [SerializeField] private float _defaultCritChance;
    [SerializeField] private float _defaultCritMultiplyer;

    public void Setup()
    {
        MinDamage = new(_minDefaultDamage);
        MaxDamage = new(_maxDefaultDamage);

        CritChance = new(_defaultCritChance);
        CritMultiplyer = new(_defaultCritMultiplyer);
    }

    public float DealBaseDamage()
    {
        var damage = Mathf.Round(Random.Range(MinDamage.ModValue, MaxDamage.ModValue));
        var chance = Random.value;

        if(chance < CritChance.ModValue)
        {
            damage *= CritMultiplyer.ModValue;
        }

        return damage;
    }
}
