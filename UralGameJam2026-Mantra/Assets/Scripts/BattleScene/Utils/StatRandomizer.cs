using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatRandomizer : MonoBehaviour, IService
{
    [SerializeField] private StatRandomizerData _data;

    public void InitUnit(Unit unit)
    {
        unit.Damage.DefaultCritChance = (float)Math.Round(Random.Range(_data.MinCritChance, _data.MaxCritChance), 2);
        unit.Damage.DefaultCritMultiplyer = (float)Math.Round(Random.Range(_data.MinCritMulti, _data.MaxCritMulti), 2);

        unit.Damage.MinDefaultDamage = (float)Math.Round(Random.Range(_data.MinMinDamage, _data.MaxMinDamage));
        unit.Damage.MaxDefaultDamage = (float)Math.Round(Random.Range(_data.MinMaxDamage, _data.MaxMaxDamage));

        unit.Health.MaxDefaultDefense = (float)Math.Round(Random.Range(_data.MinDefense, _data.MaxDefense));

        unit.Health.MaxDefaultHealth = (float)Math.Round(Random.Range(_data.MinHealth, _data.MaxHealth));

        unit.Init();
    }
}
