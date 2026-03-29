using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class UnitSelector
{
    [SerializeField] private UnitCost[] _pool;
    [SerializeField] private AnimationCurve _enemyProgress;
    [SerializeField] private Unit[] _unitPrefabs;

    public Unit RandomSelect()
    {
        var orderedPool = _pool.OrderBy(x => x.Cost).ToList();
        var currentRoom = ServiceLocator.Instance.GetService<RoomsController>().CurrentRoom;
        var sumOfCosts = _pool.Sum(x => x.Cost);

        var random = Random.Range(0, sumOfCosts);

        var cost = 0f;
        foreach (var unit in orderedPool)
        {
            cost += unit.Cost + _enemyProgress.Evaluate(currentRoom) * 5;
            if(random <= cost)
            {
                return unit.Prefab;
            }
        }

        return orderedPool[0].Prefab;
    }

    public Unit SelectUnit(UnitType unitType)
    {
        return _unitPrefabs.FirstOrDefault(x => x.UnitType == unitType);
    }
}