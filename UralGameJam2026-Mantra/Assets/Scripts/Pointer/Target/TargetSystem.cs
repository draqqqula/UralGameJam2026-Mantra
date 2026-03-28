using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetSystem : MonoBehaviour
{
    public static TargetSystem Instance;
    private MatchManager _matchManager;
    private PartyManager _partyManager;
    private RecruitingSystem _recruitingSystem;
    
    private AudioManager _audioManager;

    public Targetable Current { get; private set; }
    public event Action<Targetable> OnSetTarget;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
            _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
            _recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();
            _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        }
    }


    public bool IsTargetable(Targetable newTarget)
    {
        if (_matchManager.CurrentMatchState == MatchManager.State.Recrouting)
        {
            if (newTarget.Unit.IsMainHero || !IsGoodRecruitingTarget(newTarget)) return false;
            else return true;
        }
        else if (_matchManager.CurrentMatchState == MatchManager.State.Battle)
        {
            if (!newTarget.IsTargetable || CheckSelectingCondition(newTarget)) return false;
            else return true;
        }
        else return false;
    }

    public void TrySetTarget(Targetable newTarget)
    {
        if (newTarget == null)
        {
            if (Current != null) Current.SetTargeted(false);
            Current = newTarget;
            return;
        }
        
        if (!IsTargetable(newTarget))
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

    private bool IsGoodRecruitingTarget(Targetable target)
    {
        return (_partyManager.IsOnEnemyParty(target.Unit) && !_recruitingSystem.IsChoosingPlayerUnitToSwitch) ||
                (_partyManager.IsOnPlayerParty(target.Unit) && _recruitingSystem.IsChoosingPlayerUnitToSwitch);
    }

    public void SubmitAction()
    {
        if (Current == null) return;
        
        var battle = ServiceLocator.Instance.GetService<BattleManager>();
        var recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();

        if (_matchManager.CurrentMatchState == MatchManager.State.Battle)
        {
            if (_partyManager.IsOnPlayerParty(Current.Unit))
            {
                _audioManager.PlaySound("ChoosingUnit");
            }
            //if (battle.IsEnemyPartyMember(battle.Current.CurrentValue)) return;
            battle.TrySetUnit(Current.Unit).Forget();
        }
        else if (_matchManager.CurrentMatchState == MatchManager.State.Recrouting)
        {
            _audioManager.PlaySound("ChoosingUnit");
            recruitingSystem.ChooseUnit(Current.Unit);
        }
    }
}