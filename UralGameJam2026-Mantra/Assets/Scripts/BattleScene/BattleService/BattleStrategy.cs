using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class BattleStrategy
{
    public HashSet<Unit> _awaiableUnits = new();

    protected ReactiveProperty<Unit> _initiatorUnit = new ReactiveProperty<Unit>();
    protected ReactiveProperty<Unit> _selectedUnit = new ReactiveProperty<Unit>();
    public ReadOnlyReactiveProperty<Unit> InitiatorUnit => _initiatorUnit;
    public ReadOnlyReactiveProperty<Unit> SelectedUnit => _selectedUnit;

    protected BattleManager _battleManager;

    public BattleStrategy(HashSet<Unit> units, BattleManager battleManager)
    {
        _awaiableUnits = units;
        foreach (var unit in _awaiableUnits)
        {
            unit.UnitTurn.SetMove(true);
        }
        _battleManager = battleManager;
    }

    public virtual async UniTask TrySetUnit(Unit unit = null, Action callback = null, CancellationToken token = default)
    {
        if (_battleManager.IsEnemyTurn()) return;

        if (!unit)
        {
            callback?.Invoke();
            return;
        }

        if (!_initiatorUnit.Value && _battleManager.IsPlayerPartyMember(unit) && unit.UnitTurn.CanMove)
        {
            _initiatorUnit.Value = unit;
            return;
        }

        if (!_initiatorUnit.Value) return;

        if (_battleManager.IsEnemyPartyMember(unit) && !unit.IsAlive) return;

        RemoveTurn();

        _selectedUnit.Value = unit;
        await UseActionOn(callback, token);
    }

    public void Cancel()
    {
        _initiatorUnit.Value = null;
        _selectedUnit.Value = null;
    }

    protected virtual async UniTask UseActionOn(Action callback = null, CancellationToken token = default)
    {
        var source = _initiatorUnit.Value;
        var target = _selectedUnit.Value;

        var relationship = GetRelationship(source, target);
        switch (relationship)
        {
            case UnitRelationship.Self: await source.UpdateUltimateCooldown(target); break;
            case UnitRelationship.Friend: await source.Use<SupportAction>(target); break;
            case UnitRelationship.Enemy: 
                if(source.TryGetComponent<UnitAttackDistance>(out var distance))
                {
                    var enemies = _battleManager.GetAliveEnemyUnits(source);
                    enemies.Reverse();
                    var enemyIndex = enemies.FindIndex(x => x == target) + 1;

                    if(distance.MaxUnitDistance < enemyIndex)
                    {
                        CancelTurn(source);
                        break;
                    }
                }
                await source.Use<AttackAction>(target); 
                break;
        }

        _initiatorUnit.Value = null;
        _selectedUnit.Value = null;

        Debug.Log($"players turns left: {_awaiableUnits.Count}");

        if (!CanMoves())
        {
            callback?.Invoke();
        }

        _battleManager.CheckParty();
    }

    public void CancelTurn(Unit unit)
    {
        unit.UnitTurn.SetMove(true);
        _awaiableUnits.Add(unit);
    }

    public void RemoveTurn()
    {
        if (_initiatorUnit.Value == null) return;

        _initiatorUnit.Value.UnitTurn.SetMove(false);
        _awaiableUnits.Remove(_initiatorUnit.Value);
    }

    public UnitRelationship GetRelationship(Unit unitA, Unit unitB)
    {
        // ineffective
        if (unitA == unitB)
        {
            return UnitRelationship.Self;
        }
        else if (_battleManager.IsPlayerPartyMember(unitA) && _battleManager.IsPlayerPartyMember(unitB))
        {
            return UnitRelationship.Friend;
        }
        else if (_battleManager.IsEnemyPartyMember(unitA) && _battleManager.IsEnemyPartyMember(unitB))
        {
            return UnitRelationship.Friend;
        }
        else
        {
            return UnitRelationship.Enemy;
        }
    }

    public UnitRelationship GetRelationShipToCurrent(Unit unit)
    {
        return GetRelationship(unit, _initiatorUnit.Value);
    }

    public bool CanMoves()
    {
        return _awaiableUnits.Any();
    }
}