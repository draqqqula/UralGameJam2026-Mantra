using R3;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;

public class BattleManager : MonoBehaviour, IService
{
    public event Action OnBattleStarted;

    [SerializeField] private Party _enemyParty;
    [SerializeField] private Party _playerParty;

    private HashSet<Unit> _allUnits = new();

    public ReadOnlyReactiveProperty<Unit> InitiatorUnit => _currentPipeline?.InitiatorUnit;
    public ReadOnlyReactiveProperty<Unit> SelectedUnit => _currentPipeline?.SelectedUnit;

    private MatchManager _matchManager;
    private InfoViewController _infoViewController;

    private Turn _currentTurn = Turn.None;
    private BattleStrategy _currentPipeline;

    private CancellationTokenSource _tokenSource;
    private CancellationToken _token;

    private bool _canPlayerMove = true;
    
    public void Init()
    {
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _infoViewController = ServiceLocator.Instance.GetService<InfoViewController>();
    }

    public void Cancel()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();

        _tokenSource = new();
        _token = _tokenSource.Token;
    }

    public void SetPlayerTurn() => _currentTurn = Turn.Player;
    public void SetEnemyTurn()
    {
        _currentTurn = Turn.Bot;
        Setup();
        DetermineTurn().Forget();
    }
    public void SetNoneTurn() => _currentTurn = Turn.None;

    public void InitializeBattle()
    {
        SetPlayerTurn();

        _infoViewController.Show();
        _enemyParty.Members.Reverse();
        _matchManager.CurrentMatchState = MatchManager.State.Battle;

        OnBattleStarted?.Invoke();
        Setup();
        DetermineTurn().Forget();
    }

    public async UniTaskVoid TrySetUnit(Unit unit)
    {
        if (IsNoneTurn() || _currentPipeline == null || !_canPlayerMove || _currentPipeline is EnemyBattleStrategy) return;

        Action callback = () =>
        {
            _currentTurn = ReverseTurn();

            Setup();
            CheckBattlefield();
        };

        _canPlayerMove = false;

        await _currentPipeline.TrySetUnit(unit, callback, _token);

        _canPlayerMove = true;
    }


    private void CheckBattlefield()
    {
        CheckParty();

        DetermineTurn().Forget();
    }

    private void UpdateModifiers(List<Unit> units)
    {
        foreach (var unit in units)
        {
            unit.CheckAttached();

            unit.Damage.CritMultiplyer.CheckModifiers();
            unit.Damage.CritChance.CheckModifiers();

            unit.Damage.MaxDamage.CheckModifiers();
            unit.Damage.MinDamage.CheckModifiers();
        }
    }

    private void Setup()
    {
        _allUnits.Clear();
        
        foreach(var unit in _playerParty.Members)
        {
            if (!unit.IsAlive) _allUnits.Remove(unit);
            unit.UpdateUIPosition();
            unit.ShowHealthbars();

            _allUnits.Add(unit);
        }

        foreach (var unit in _enemyParty.Members)
        {
            if (!unit.IsAlive) _allUnits.Remove(unit);
            unit.UpdateUIPosition();
            unit.ShowHealthbars();

            _allUnits.Add(unit);
        }
    }

    private async UniTaskVoid DetermineTurn()
    {
        if (IsNoneTurn()) return;

        UpdateModifiers(_allUnits.ToList());

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

    private Turn ReverseTurn()
    {
        var turn = _currentTurn == Turn.Player ? Turn.Bot : Turn.Player;

        return turn;
    }

    public List<Unit> GetFriends(Unit source)
    {
        var units = new List<Unit>();
        if (IsPlayerTurn())
        {
            units = _playerParty.Members;
        }
        if (IsEnemyTurn())
        {
            units = _enemyParty.Members;
        }

        return units;
    }

    public List<Unit> GetAliveFriendUnits(Unit source)
    {
        var units = new List<Unit>();
        if (IsPlayerTurn())
        {
            units = _playerParty.Members.Where(x => x.IsAlive).ToList();
        }
        if(IsEnemyTurn())
        {
            units = _enemyParty.Members.Where(x => x.IsAlive).ToList();
        }

        return units;
    }

    public List<Unit> GetAliveEnemyUnits(Unit source)
    {
        var units = new List<Unit>();
        if (IsPlayerTurn())
        {
            units = _enemyParty.Members.Where(x => x.IsAlive).ToList();
        }
        if (IsEnemyTurn())
        {
            units = _playerParty.Members.Where(x => x.IsAlive).ToList();
        }

        return units;
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
        if (IsNoneTurn()) return;

        Action callback = () =>
        {
            _currentTurn = ReverseTurn();
            _currentPipeline = null;

            CheckBattlefield();
        };

        await _currentPipeline.TrySetUnit(callback: callback, token: _token);

    }

    public void CheckParty(Action callback = null)
    {
        var units = GetAliveUnits();

        var playerUnits = units.Any(x => IsPlayerPartyMember(x));
        var enemyUnits = units.Any(x => IsEnemyPartyMember(x));

        Cancel();

        if (!playerUnits)
        {
            _currentPipeline = null;
            _matchManager.DeclareDefeat();

            _infoViewController.Hide();

            SetNoneTurn();

            callback?.Invoke();

            return;
        }

        if (!enemyUnits)
        {
            _currentPipeline = null;
            _matchManager.DeclareVictory();

            SetNoneTurn();

            callback?.Invoke();

            return;
        }
    }

    public bool IsPlayerTurn()
    {
        return _currentTurn == Turn.Player;
    }

    public bool IsEnemyTurn()
    {
        return _currentTurn == Turn.Bot;
    }

    public bool IsNoneTurn()
    {
        return _currentTurn == Turn.None;
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

    public Unit GetCurrentUnit()
    {
        if (_currentPipeline == null) return null;

        return _currentPipeline.InitiatorUnit.CurrentValue;
    }

    public bool IsSelecting()
    {
        return GetCurrentUnit() == null;
    }

    private void OnDestroy()
    {
        Cancel();
    }
}

public enum Turn
{
    Player,
    Bot,
    None
}