using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class TurnManager : MonoBehaviour, IService
{
    public CancellationToken Token => _cancelToken;

    private Stack<(Unit, UnitAction)> _turns = new();
    private CancellationTokenSource _cancelSource;
    private CancellationToken _cancelToken;

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

    public (Unit unit, UnitAction action) CancelTurn(ref ReactiveProperty<Unit> currentUnit, ref Queue<Unit> unitOrder)
    {
        var current = currentUnit.Value;

        (Unit unit, UnitAction action) = GetPreviousTurn();

        if (unit == null) return (null, null);

        if (TestBattleManager.Instance.IsEnemyPartyMember(unit))
        {
            return (null, null);
        }

        currentUnit.Value = unit;
        if (action != null)
        {
            action.Undo();
        }

        var order = new Queue<Unit>(unitOrder);

        unitOrder.Clear();

        unitOrder.Enqueue(current);
        //_unitOrder.Enqueue(_currentUnit.Value);

        foreach (var unitNewOrder in order)
        {
            unitOrder.Enqueue(unitNewOrder);
        }

        return (unit, action);
    }

    public async UniTask ExecutePlayerTurns()
    {
        if (!_turns.Any()) return;

        var rightOrder = _turns.Reverse();

        foreach (var turn in rightOrder)
        {
            if (turn.Item2.CanUse())
            {
                await turn.Item2.Execute(_cancelToken);
            }
        }

        _turns.Clear();
    }

    public void SkipTurn(ref ReactiveProperty<Unit> currentUnit)
    {
        if (TestBattleManager.Instance.IsEnemyPartyMember(currentUnit.Value)) return;

        AddTurn(currentUnit.Value, null);

        TestBattleManager.Instance.UpdateOrder();
    }

    public void Cancel()
    {
        _cancelSource?.Cancel();

        _cancelSource?.Dispose();
        _cancelSource = new();
        _cancelToken = _cancelSource.Token;
    }
}
