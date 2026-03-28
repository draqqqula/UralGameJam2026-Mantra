using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class RecruitingSystem : MonoBehaviour, IService
{
    private PartyManager _partyManager;
    private AudioManager _audioManager;
    
    public bool IsChoosingPlayerUnitToSwitch {get; private set;}
    
    private Unit _chosenEnemyUnit;
    
    [SerializeField] private AnimationCurve _fadeOutCurve;
    [SerializeField] private AnimationCurve _moveCurve;
    
    [SerializeField] private float _fadeOutDuration = 1;
    [SerializeField] private float _moveSpeed = 5;
    
    private List<RecruitingAnimation> _animations = new List<RecruitingAnimation>();
    private List<Unit> _movingUnits = new List<Unit>();
    
    public event Action OnUnitChoosed;
    
    private void Awake()
    {
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
    }

    public void ChooseUnit(Unit unit)
    {
        TargetSystem.Instance.TrySetTarget(null);
        
        if (IsChoosingPlayerUnitToSwitch) OnPlayerUnitChosen(unit);
        else OnEnemyUnitChosen(unit);
        
        OnUnitChoosed?.Invoke();
    }
    
    private void OnEnemyUnitChosen(Unit unit)
    {
        if (_partyManager.PlayerParty.Members.Count + _movingUnits.Count < _partyManager.PlayerParty.MaxCount)
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
        OnBeforeRecruit(unit);
        
        Action onAnimationFinished = () =>
        {
            _partyManager.PlayerParty.AddMember(unit);
            
            unit.transform.rotation = Quaternion.identity;
            //unit.InstantiateAura();
            unit.UpdateRenderCameraPoint();
            unit.UpdateUIPosition();
            _movingUnits.Remove(unit);
        };
        
        var animation = new RecruitingAnimation(_partyManager.PlayerPartyPlacer, _audioManager);
        animation.Play(unit, _movingUnits.Count, _moveSpeed, _moveCurve, onAnimationFinished);
        _movingUnits.Add(unit);
        _animations.Add(animation);
    }

    public void RecruitUnitWithReplacement(Unit oldUnit, Unit newUnit)
    {
        var index = _partyManager.PlayerParty.IndexOfMember(oldUnit) + _movingUnits.Count;
        _partyManager.PlayerParty.RemoveMember(oldUnit);
        OnBeforeRecruit(newUnit);
        
        Action onAnimationFinished = () =>
        {
            Destroy(oldUnit.gameObject);
            _partyManager.PlayerParty.InsertMember(index, newUnit);
            
            newUnit.transform.rotation = Quaternion.identity;
            //newUnit.InstantiateAura();
            newUnit.UpdateRenderCameraPoint();
            newUnit.UpdateUIPosition();
            _movingUnits.Remove(newUnit);
        };
        
        var animation = new RecruitingAnimation(_partyManager.PlayerPartyPlacer, _audioManager);
        animation.PlayWithReplacement(oldUnit, newUnit, _moveSpeed, _fadeOutDuration, _fadeOutCurve, _moveCurve, onAnimationFinished);
        _audioManager.PlaySound("Recruiting");
        
        _movingUnits.Add(newUnit);
        _animations.Add(animation);
        
        IsChoosingPlayerUnitToSwitch = false;
        _chosenEnemyUnit = null;
    }

    private void OnBeforeRecruit(Unit newUnit)
    {
        _partyManager.EnemyParty.RemoveMember(newUnit);
        newUnit.Resurrect();
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
    
    private AudioManager _audioManager;
    
    public RecruitingAnimation(PartyPlacer playerPartyPlacer, AudioManager audioManager)
    {
        _playerPartyPlacer = playerPartyPlacer;
        _audioManager = audioManager;
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

    public void Play(Unit newUnit, int movingUnitsCount, float moveSpeed, AnimationCurve moveCurve, Action callback = null)
    {
        var targetPos = GetPlayerPartyTargetPos(newUnit, movingUnitsCount);
        
        _animationSequence = DOTween.Sequence();
        _animationSequence.Append(PlayMovePart(newUnit, targetPos, moveSpeed, moveCurve));
        _animationSequence.OnKill(() => callback?.Invoke());
    }

    private Vector2 GetPlayerPartyTargetPos(Unit newUnit, int movingUnitsCount)
    {
        var startPos = newUnit.transform.position;
        _playerPartyPlacer.PlaceInParty(newUnit, movingUnitsCount + 1);
        var targetPos = newUnit.transform.position;
        newUnit.transform.position = startPos;
        return targetPos;
    }

    private Tween PlayMovePart(Unit newUnit, Vector2 playerUnitPos, float moveSpeed, AnimationCurve moveCurve)
    {
        var distance = Vector2.Distance(playerUnitPos, newUnit.transform.position);
        return newUnit.transform
            .DOMove(playerUnitPos, distance / moveSpeed)
            .SetEase(moveCurve)
            .OnComplete(() => _audioManager.PlaySound("Step"));
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