using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class EnemyBattleStrategy : BattleStrategy
{
    public EnemyBattleStrategy(HashSet<Unit> units, BattleManager battleManager) : base(units, battleManager)
    {
        foreach(Unit unit in units)
        {
            unit.UnitTurn.SetMove(true);
        }
    }

    public override async UniTask TrySetUnit(Unit unit = null, Action callback = null, CancellationToken token = default)
    {
        await DoSetUnit(callback, token);
    }

    private async UniTask DoSetUnit(Action callback = null, CancellationToken token = default)
    {
        while (CanMoves())
        {
            var canMoving = _awaiableUnits.Where(x => x.UnitTurn.CanMove);
            var aliveUnits = _battleManager.GetAliveUnits();

            var indexSource = Random.Range(0, canMoving.Count());
            var indexTarget = Random.Range(0, aliveUnits.Count);

            var target = aliveUnits.ElementAt(indexTarget);
            var source = canMoving.ElementAt(indexSource);

            source.UnitTurn.SetMove(false);
            _awaiableUnits.Remove(source);

            _initiatorUnit.Value = source;
            _selectedUnit.Value = target;

            await UseActionOn(callback, token);
        }
    }

    protected override async UniTask UseActionOn(Action callback = null, CancellationToken token = default)
    {
        if (_initiatorUnit.Value == null || _selectedUnit.Value == null) return;

        var source = _initiatorUnit.Value;
        var target = _selectedUnit.Value;

        var relationship = GetRelationship(source, target);
        switch (relationship)
        {
            case UnitRelationship.Self: await source.UpdateUltimateCooldown(target); break;
            case UnitRelationship.Friend: await source.Use<SupportAction>(target); break;
            case UnitRelationship.Enemy:
                if (source.TryGetComponent<UnitAttackDistance>(out var distance))
                {
                    var enemies = _battleManager.GetAliveEnemyUnits(source);
                    var enemyIndex = enemies.FindIndex(x => x == target) + 1;

                    if (distance.MaxUnitDistance < enemyIndex)
                    {
                        target = TryGetAnotherEnemy(source, distance.MaxUnitDistance);
                        break;
                    }
                }
                await source.Use<AttackAction>(target); 
                break;
        }

        _initiatorUnit.Value = null;
        _selectedUnit.Value = null;

        Debug.Log($"enemy turns left: {_awaiableUnits.Count}");

        if (!CanMoves())
        {
            callback?.Invoke();
        }

        _battleManager.CheckParty();
    }

    private Unit TryGetAnotherEnemy(Unit source, int maxDistance)
    {
        var enemies = _battleManager.GetAliveEnemyUnits(source);
        var alive = enemies.Where(x => x.IsAlive).ToList();

        var target = alive[Random.Range(0, maxDistance)];

        //var enemyIndex = enemies.FindIndex(x => x == target) + 1;

        //if (maxDistance < enemyIndex)
        //{
        //    target = TryGetAnotherEnemy(source, maxDistance);
        //}

        return target;
    }
}