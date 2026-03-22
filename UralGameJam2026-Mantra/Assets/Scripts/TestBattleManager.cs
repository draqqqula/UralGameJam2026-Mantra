using System;
using R3;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;

public class TestBattleManager : MonoBehaviour, IService
{
    public static TestBattleManager Instance;

    public bool IsEnemyTurn;

    [SerializeField] private Party _playersUnits;
    [SerializeField] private Party _enemiesUnits;
    
    public Party PlayersUnits => _playersUnits;
    public Party EnemiesUnits => _enemiesUnits;

    private Queue<Unit> _unitOrder = new();
    private HashSet<Unit> _allUnits = new();

    private ReactiveProperty<Unit> _currentUnit = new ReactiveProperty<Unit>();
    public ReadOnlyReactiveProperty<Unit> Current => _currentUnit;

    private MatchManager _matchManager;
    private PartyManager _partyManager;
    private TurnManager _turnManager;
    
    public event Action OnBattleStarted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void Init()
    {
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _turnManager = ServiceLocator.Instance.GetService<TurnManager>();
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
            _turnManager.CancelTurn(ref _currentUnit, ref _unitOrder);
        }
#endif
    }
    
    public void InitializeBattle()
    {
        _enemiesUnits.Members.Reverse();

        OnBattleStarted?.Invoke();
        Setup();
    }

    private void Setup()
    {
        _allUnits.Clear();
        _unitOrder.Clear();
        
        _currentUnit.Value = _playersUnits.Members[0];
        foreach (var unit in _playersUnits.Members)
        {
            unit.UpdateHealthbarPosition();
            unit.ShowHealthbars();
            
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

    public bool UnitIsCurrent(Unit unit)
    {
        return _currentUnit.Value == unit;
    }

    public Unit GetRandomEnemy()
    {
        return _enemiesUnits.Members[Random.Range(0, _enemiesUnits.Members.Count)];
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
        return GetRelationship(unit, _currentUnit.Value);
    }

    public void UseActionOn(Unit source, Unit target)
    {
        if (!UnitIsCurrent(source)) return;

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
            _currentUnit.Value = null;
            _matchManager.DeclareDefeat();
            
            return;
        }

        if (!enemyUnits)
        {
            print("player won");
            _currentUnit.Value = null;
            _matchManager.DeclareVictory();
            
            return;
        }

        if (units.TryDequeue(out var unit))
        {
            _currentUnit.Value = unit;
            return;
        }
        else
        {
            Setup();
            return;
        }
    }

    private async UniTask DetermineTurn()
    {
        if (IsPlayerPartyMember(_currentUnit.Value))
        {
            IsEnemyTurn = false;
        }

        if (IsEnemyPartyMember(_currentUnit.Value))
        {
            if (!_currentUnit.Value.IsAlive)
            {
                _turnManager.Cancel();
                UpdateOrder();
                return;
            }
            IsEnemyTurn = true;

            await _turnManager.ExecutePlayerTurns();

            int randomIndex = Random.Range(0, _allUnits.Count);
            var target = _allUnits.ElementAt(randomIndex);

            var relation = GetRelationShipToCurrent(target);

            UnitAction action = relation switch
            {
                UnitRelationship.Friend => _currentUnit.Value.Get<SupportAction>(),
                UnitRelationship.Enemy => _currentUnit.Value.Get<AttackAction>(),
                _ => _currentUnit.Value.Get<UltimateAttackAction>(),
            };

            action.Plan(_currentUnit.Value, target);
            if (action.CanUse())
            {
                await action.Execute(_turnManager.Token);
            }
            if (action is UltimateAttackAction ultimate) ultimate.DecreaseCooldown();

            UpdateOrder();
        }
    }

    
}
