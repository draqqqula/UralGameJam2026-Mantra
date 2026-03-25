using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetSystem : MonoBehaviour
{
    public static TargetSystem Instance;

    public Targetable Current { get; private set; }
    public event Action<Targetable> OnSetTarget;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void TrySetTarget(Targetable newTarget)
    {
        if (!newTarget.IsTargetable)
        {
            return;
        }

        if (CheckSelectingCondition(newTarget))
        {
            return;
        }

        if (Current != null)
        {
            Current.SetTargeted(false);
        }

        Current = newTarget;
        Current.SetTargeted(true);
        OnSetTarget?.Invoke(Current);
    }

    private bool CheckSelectingCondition(Targetable newTarget)
    {
        var battle = ServiceLocator.Instance.GetService<BattleManager>();
        var selecting = battle.IsSelecting();
        if (selecting)
        {
            var partyMember = battle.IsPlayerPartyMember(newTarget.Unit);
            var canMove = newTarget.Unit.UnitTurn.CanMove;
            return !(partyMember && canMove);
        }
        return false;
    }

    public void SubmitAction()
    {
        var battle = ServiceLocator.Instance.GetService<BattleManager>();

        if (battle.IsEnemyPartyMember(battle.Current.CurrentValue)) return;

        battle.TrySetUnit(Current.Unit).Forget();
    }
}