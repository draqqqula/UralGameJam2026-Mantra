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

        if (!unit.UnitTurn.CanMove) return;
        if (!unit.IsAlive) return;

        if (_initiatorUnit.Value == null && _battleManager.IsPlayerPartyMember(unit))
        {
            _initiatorUnit.Value = unit;
            return;
        }

        _selectedUnit.Value = unit;
        await UseActionOn(callback, token);
    }

    protected virtual async UniTask UseActionOn(Action callback = null, CancellationToken token = default)
    {
        var source = _initiatorUnit.Value;
        var target = _selectedUnit.Value;

        source.UnitTurn.SetMove(false);
        _awaiableUnits.Remove(source);

        var relationship = GetRelationship(source, target);
        switch (relationship)
        {
            case UnitRelationship.Self: await source.UpdateUltimateCooldown(target, token); break;
            case UnitRelationship.Friend: await source.Use<SupportAction>(target, token); break;
            case UnitRelationship.Enemy: await source.Use<AttackAction>(target, token); break;
        }

        _initiatorUnit.Value = null;
        _selectedUnit.Value = null;

        Debug.Log($"players turns left: {_awaiableUnits.Count}");

        if (!CanMoves())
        {
            callback?.Invoke();
        }
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