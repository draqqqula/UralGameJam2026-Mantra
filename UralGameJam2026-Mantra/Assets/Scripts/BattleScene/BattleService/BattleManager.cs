using R3;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BattleManager : MonoBehaviour, IService
{
    public event Action OnBattleStarted;

    [SerializeField] private Party _enemyParty;
    [SerializeField] private Party _playerParty;

    private HashSet<Unit> _allUnits = new();

    private ReactiveProperty<Unit> _currentUnit = new ReactiveProperty<Unit>();
    public ReadOnlyReactiveProperty<Unit> Current => _currentUnit;

    private MatchManager _matchManager;
    private PartyManager _partyManager;

    private Turn _currentTurn;
    private BattleStrategy _currentPipeline;

    private CancellationTokenSource _tokenSource;
    private CancellationToken _token;

    public void Init()
    {
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();

        InitializeFirstBattle();
    }

    public void Cancel()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();

        _tokenSource = new();
        _token = _tokenSource.Token;
    }

    public void InitializeFirstBattle()
    {
        _currentTurn = Turn.Player;

        Action callback = () =>
        {
            _enemyParty.Members.Reverse();

            OnBattleStarted?.Invoke();

            Setup();
            DetermineTurn().Forget();
        };

        _partyManager.InitializeEnemyParty(4);
        _partyManager.InitializePlayerParty(4);
    }

    public void InitializeBattle()
    {
        _currentTurn = Turn.Player;

        Action callback = () =>
        {
            _enemyParty.Members.Reverse();

            OnBattleStarted?.Invoke();
            Setup();
        };

        _partyManager.PlacePlayerParty(callback);
        _partyManager.InitializeEnemyParty(4);
    }

    public void TrySetUnit(Unit unit)
    {
        if (_currentPipeline == null) return;

        Action callback = () =>
        {
            _currentTurn = _currentTurn == Turn.Player ? Turn.Bot : Turn.Player;

            Setup();
            CheckBattlefield();
        };

        _currentPipeline.TrySetUnit(unit, callback, _token);
    }

    private void CheckBattlefield()
    {
        var units = GetAliveUnits();

        var playerUnits = units.Any(x => IsPlayerPartyMember(x));
        var enemyUnits = units.Any(x => IsEnemyPartyMember(x));

        if (!playerUnits)
        {
            print("enemy won");
            _currentPipeline = null;
            _matchManager.DeclareDefeat();

            return;
        }

        if (!enemyUnits)
        {
            print("player won");
            _currentPipeline = null;
            _matchManager.DeclareVictory();

            return;
        }

        DetermineTurn().Forget();
    }

    private void Setup()
    {
        _allUnits.Clear();
        
        foreach(var unit in _playerParty.Members)
        {
            if (!unit.IsAlive) _allUnits.Remove(unit);
            unit.UpdateUIPosition();

            _allUnits.Add(unit);
        }

        foreach (var unit in _enemyParty.Members)
        {
            if (!unit.IsAlive) _allUnits.Remove(unit);
            unit.UpdateUIPosition();

            _allUnits.Add(unit);
        }
    }

    private async UniTaskVoid DetermineTurn()
    {
        if(IsPlayerTurn())
        {
            var alive = _playerParty.Members.Where(x => x.IsAlive).ToList();
            var set = new HashSet<Unit>(alive);

            _currentPipeline = new BattleStrategy(set, this);
            return;
        }

        if (IsEnemyTurn())
        {
            var alive = _enemyParty.Members.Where(x => x.IsAlive).ToList();
            var set = new HashSet<Unit>(alive);

            _currentPipeline = new EnemyBattleStrategy(set, this);
            await DoBotMove();
        }
    }

    public List<Unit> GetAliveUnits()
    {
        var units = _allUnits.Where(x => x.IsAlive).ToList();

        return units;
    }

    public Unit GetRandomEnemy()
    {
        var alive = _enemyParty.Members.Where(x => x.IsAlive).ToList();

        var unit = alive[Random.Range(0, alive.Count())];
        return unit;
    }

    private async UniTask DoBotMove()
    {
        Action callback = () =>
        {
            _currentTurn = _currentTurn == Turn.Player ? Turn.Bot : Turn.Player;
            _currentPipeline = null;

            CheckBattlefield();
        };

        await _currentPipeline.TrySetUnit(callback: callback, token: _token);

    }

    public bool IsPlayerTurn()
    {
        return _currentTurn == Turn.Player;
    }

    public bool IsEnemyTurn()
    {
        return _currentTurn == Turn.Bot;
    }

    public bool IsPlayerPartyMember(Unit target)
    {
        return _playerParty.Members.Contains(target) && target != null;
    }

    public bool IsEnemyPartyMember(Unit target)
    {
        return _enemyParty.Members.Contains(target) && target != null;
    }

    public UnitRelationship GetRelationShipToCurrent(Unit unit)
    {
        if (_currentPipeline == null) return UnitRelationship.None;

        return _currentPipeline.GetRelationShipToCurrent(unit);
    }
}

public enum Turn
{
    Player,
    Bot,
    None
}