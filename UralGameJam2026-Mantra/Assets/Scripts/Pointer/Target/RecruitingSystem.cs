using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class RecruitingSystem : MonoBehaviour, IService
{
    private PartyManager _partyManager;
    
    public bool IsChoosingPlayerUnitToSwitch {get; private set;}
    private Unit _chosenEnemyUnit;
    
    [SerializeField] private AnimationCurve _fadeOutCurve;
    [SerializeField] private AnimationCurve _moveCurve;
    
    private List<RecruitingAnimation> _animations = new List<RecruitingAnimation>();
    
    private void Awake()
    {
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
    }

    public void ChooseUnit(Unit unit)
    {
        TargetSystem.Instance.TrySetTarget(null);
        
        if (IsChoosingPlayerUnitToSwitch) OnPlayerUnitChosen(unit);
        else OnEnemyUnitChosen(unit);
    }
    
    private void OnEnemyUnitChosen(Unit unit)
    {
        if (_partyManager.PlayerParty.Members.Count < _partyManager.PlayerParty.MaxCount)
        {
            RecruitUnit(unit);
        }
        else
        {
            IsChoosingPlayerUnitToSwitch = true;
            _chosenEnemyUnit = unit;
        }
    }
    
    private void OnPlayerUnitChosen(Unit unit)
    {
        RecruitUnitWithReplacement(unit, _chosenEnemyUnit);
    }
    
    public void RecruitUnit(Unit unit)
    {
        _partyManager.EnemyParty.RemoveMember(unit);
        unit.Health.ApplyHeal(unit.Health.MaxHealth);
        unit.GetComponent<UnitAnimator>()?.Play(UnitAnimation.Idle, out _);
        unit.GetComponent<UnitRetired>()?.Resurrect();
        
        Action onAnimationFinished = () =>
        {
            _partyManager.PlayerParty.AddMember(unit);
            
            unit.transform.rotation = Quaternion.identity;
            unit.InstantiateAura();
            unit.UpdateRenderCameraPoint();
            unit.UpdateUIPosition();
        };
        

        var animation = new RecruitingAnimation(_partyManager.PlayerPartyPlacer);
        animation.Play(unit, 5, _moveCurve, onAnimationFinished);
        _animations.Add(animation);
    }

    public void RecruitUnitWithReplacement(Unit oldUnit, Unit newUnit)
    {
        _partyManager.EnemyParty.RemoveMember(newUnit);
        newUnit.Health.ApplyHeal(newUnit.Health.MaxHealth);
        newUnit.GetComponent<UnitAnimator>()?.Play(UnitAnimation.Idle, out _);
        newUnit.GetComponent<UnitRetired>()?.Resurrect();
        
        Action onAnimationFinished = () =>
        {
            var index = _partyManager.PlayerParty.IndexOfMember(oldUnit);
            _partyManager.PlayerParty.DestroyMember(oldUnit);
            _partyManager.PlayerParty.InsertMember(index, newUnit);
            
            newUnit.transform.rotation = Quaternion.identity;
            newUnit.InstantiateAura();
            newUnit.UpdateRenderCameraPoint();
            newUnit.UpdateUIPosition();
        };
        
        var animation = new RecruitingAnimation(_partyManager.PlayerPartyPlacer);
        animation.PlayWithReplacement(oldUnit, newUnit, 5,1, _fadeOutCurve, _moveCurve, onAnimationFinished);
        _animations.Add(animation);
        
        IsChoosingPlayerUnitToSwitch = false;
        _chosenEnemyUnit = null;
    }

    public void KillAllAnimations()
    {
        foreach (var animation in _animations)
        {
            animation?.Kill();
        }
        _animations.Clear();
    }

    private void OnDestroy()
    {
        KillAllAnimations();
    }
}

public class RecruitingAnimation
{
    private Sequence _animationSequence;
    private PartyPlacer _playerPartyPlacer;
    
    public RecruitingAnimation(PartyPlacer playerPartyPlacer)
    {
        _playerPartyPlacer = playerPartyPlacer;
    }
    
    public void PlayWithReplacement(Unit oldUnit, Unit newUnit, float moveSpeed, float fadeOutDuration,
        AnimationCurve fadeOutCurve, AnimationCurve moveCurve, Action callback = null)
    {
        _animationSequence = DOTween.Sequence();
        newUnit.transform.SetParent(oldUnit.transform.parent);
        
        var oldUnitAnimator = oldUnit.GetComponent<UnitAnimator>();
        _animationSequence.Append(oldUnitAnimator.PlayFadeOutAnimation(fadeOutDuration, fadeOutCurve));
        _animationSequence.Append(PlayMovePart(newUnit, oldUnit.transform.position, moveSpeed, moveCurve));
        _animationSequence.OnKill(() => callback?.Invoke());
    }

    public void Play(Unit newUnit, float moveSpeed, AnimationCurve moveCurve, Action callback = null)
    {
        var targetPos = GetPlayerPartyTargetPos(newUnit);
        
        _animationSequence = DOTween.Sequence();
        _animationSequence.Append(PlayMovePart(newUnit, targetPos, moveSpeed, moveCurve));
        _animationSequence.OnKill(() => callback?.Invoke());
    }

    private Vector2 GetPlayerPartyTargetPos(Unit newUnit)
    {
        var startPos = newUnit.transform.position;
        _playerPartyPlacer.PlaceInParty(newUnit);
        var targetPos = newUnit.transform.position;
        newUnit.transform.position = startPos;
        return targetPos;
    }

    private Tween PlayMovePart(Unit newUnit, Vector2 playerUnitPos, float moveSpeed, AnimationCurve moveCurve)
    {
        var distance = Vector2.Distance(playerUnitPos, newUnit.transform.position);
        return newUnit.transform.DOMove(playerUnitPos, distance / moveSpeed).SetEase(moveCurve);
    }

    public void Kill()
    {
        if (_animationSequence != null)
        {
            _animationSequence.Kill();
            _animationSequence = null;
        }
    }
}