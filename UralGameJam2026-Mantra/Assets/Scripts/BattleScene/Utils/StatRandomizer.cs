using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatRandomizer : MonoBehaviour, IService
{
    [SerializeField] private StatRandomizerData[] _datas;

    public void InitUnit(Unit unit)
    {
        var data = _datas.FirstOrDefault(x => x.UnitType == unit.UnitType);

        unit.Damage.DefaultCritChance = (float)Math.Round(Random.Range(data.MinCritChance, data.MaxCritChance), 2);
        unit.Damage.DefaultCritMultiplyer = (float)Math.Round(Random.Range(data.MinCritMulti, data.MaxCritMulti), 2);

        unit.Damage.MinDefaultDamage = (float)Math.Round(Random.Range(data.MinMinDamage, data.MaxMinDamage));
        unit.Damage.MaxDefaultDamage = (float)Math.Round(Random.Range(data.MinMaxDamage, data.MaxMaxDamage));

        unit.Health.MaxDefaultDefense = (float)Math.Round(Random.Range(data.MinDefense, data.MaxDefense));

        unit.Health.MaxDefaultHealth = (float)Math.Round(Random.Range(data.MinHealth, data.MaxHealth));

        unit.Variant = Random.Range(0, data.VariantCount);

        unit.Init();
    }
}
