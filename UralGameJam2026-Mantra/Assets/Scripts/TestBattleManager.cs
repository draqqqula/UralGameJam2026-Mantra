using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestBattleManager : MonoBehaviour
{
    public static TestBattleManager Instance;

    [SerializeField] private Party _playersUnits;
    [SerializeField] private Party _enemiesUnits;

    private Queue<Unit> _unitOrder = new();
    private Stack<(Unit, UnitAction)> _turns = new();
    private HashSet<Unit> _allUnits = new();

    private Unit _currentUnit;

    public Unit Current => _currentUnit;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializeBattle();
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdateOrder();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CancelTurn();
        }
#endif
    }

    public void InitializeBattle()
    {
        //here game will load enemy and player party from PartyManager and start battle, propably

        Setup();
    }

    private void Setup()
    {
        foreach (var unit in _playersUnits.Members)
        {
            if (!unit.IsAlive)
            {
                _allUnits.Remove(unit);
                continue;
            }
            _allUnits.Add(unit);

            _unitOrder.Enqueue(unit);
        }

        foreach(var unit in _enemiesUnits.Members)
        {
            if (!unit.IsAlive)
            {
                _allUnits.Remove(unit);
                continue;
            }
            _allUnits.Add(unit);
            _unitOrder.Enqueue(unit);
        }

        UpdateOrder();
    }

    public void UpdateOrder()
    {
        UpdateModifiers(_unitOrder);

        CheckBattlefield(_allUnits, _unitOrder);

        DetermineTurn();
    }

    public void AddTurn(Unit unit, UnitAction action)
    {
        _turns.Push((unit, action));
    }

    public (Unit unit, UnitAction action) GetPreviousTurn()
    {
        if (!_turns.TryPeek(out var _))
        {
            return (null, null);
        }

        (Unit unit, UnitAction action) = _turns.Pop();
        return (unit, action);
    }

    public (Unit unit, UnitAction action) CancelTurn()
    {
        var current = _currentUnit;

        (Unit unit, UnitAction action) = GetPreviousTurn();

        if(unit == null) return (null, null);

        if (IsEnemyPartyMember(unit))
        {
            return (null, null);
        }

        _currentUnit = unit;
        if (action != null)
        {
            action.Undo();
        }

        var order = new Queue<Unit>(_unitOrder);

        _unitOrder.Clear();

        _unitOrder.Enqueue(current);
        _unitOrder.Enqueue(_currentUnit);

        foreach(var unitOrder in order)
        {
            _unitOrder.Enqueue(unitOrder);
        }

        return (unit, action);
    }

    public bool UnitIsCurrent(Unit unit)
    {
        return _currentUnit == unit;
    }

    public UnitRelationship GetRelationship(Unit unitA, Unit unitB)
    {
        // ineffective
        if (unitA == unitB)
        {
            return UnitRelationship.Self;
        }
        else if (IsPlayerPartyMember(unitA) && IsPlayerPartyMember(unitB))
        {
            return UnitRelationship.Friend;
        }
        else if (IsEnemyPartyMember(unitA) && IsEnemyPartyMember(unitB))
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
        return GetRelationship(unit, _currentUnit);
    }

    public void UseActionOn(Unit source, Unit target)
    {
        var relationship = GetRelationship(source, target);
        switch (relationship)
        {
            case UnitRelationship.Self: source.UpdateUltimateCooldown(); break;
            case UnitRelationship.Friend: source.Use<SupportAction>(target); break;
            case UnitRelationship.Enemy: source.Use<AttackAction>(target); break;
        }
    }

    public bool IsPlayerPartyMember(Unit target)
    {
        return _playersUnits.Members.Contains(target) && target != null;
    }

    public bool IsEnemyPartyMember(Unit target)
    {
        return _enemiesUnits.Members.Contains(target) && target != null;
    }

    private void UpdateModifiers(Queue<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.Damage.CritMultiplyer.CheckModifiers();
            unit.Damage.CritChance.CheckModifiers();

            unit.Damage.MaxDamage.CheckModifiers();
            unit.Damage.MinDamage.CheckModifiers();
        }
    }

    private void CheckBattlefield(IEnumerable<Unit> unitsExists, Queue<Unit> units)
    {
        print($"left:{unitsExists.Count()}");

        var playerUnits = unitsExists.Any(x => IsPlayerPartyMember(x));
        var enemyUnits = unitsExists.Any(x => IsEnemyPartyMember(x));

        if (!playerUnits)
        {
            print("enemy won");
            _currentUnit = null;

            return;
        }

        if (!enemyUnits)
        {
            print("player won");
            _currentUnit = null;

            return;
        }

        if (!units.TryDequeue(out _currentUnit))
        {
            Setup();
            return;
        }
    }

    private void DetermineTurn()
    {
        if (IsEnemyPartyMember(_currentUnit))
        {
            ExecutePlayerTurns();

            var action = _currentUnit.UnitActions[Random.Range(0, _currentUnit.UnitActions.Count)];
            var target = _playersUnits.Members[Random.Range(0, _playersUnits.Members.Count)];

            action.Plan(_currentUnit, target);
            action.Execute();

            UpdateOrder();
        }
    }

    private void ExecutePlayerTurns()
    {
        if (!_turns.Any()) return;

        foreach(var turn in _turns)
        {
            turn.Item2.Execute();
        }

        _turns.Clear();
    }
}
