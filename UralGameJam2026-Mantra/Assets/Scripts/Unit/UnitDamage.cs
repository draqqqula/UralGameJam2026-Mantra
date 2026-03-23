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

    public float MinDefaultDamage;
    public float MaxDefaultDamage;
    [Space(10)]

    public float DefaultCritChance;
    public float DefaultCritMultiplyer;

    public void Setup()
    {
        MinDamage = new(MinDefaultDamage);
        MaxDamage = new(MaxDefaultDamage);

        CritChance = new(DefaultCritChance);
        CritMultiplyer = new(DefaultCritMultiplyer);
    }

    public float DealBaseDamage()
    {
        var damage = Mathf.Round(Random.Range(MinDamage.ModValue, MaxDamage.ModValue));
        var chance = Random.value;

        if(chance < CritChance.ModValue)
        {
            damage *= CritMultiplyer.ModValue;
        }

        return Mathf.Round(damage);
    }
}
