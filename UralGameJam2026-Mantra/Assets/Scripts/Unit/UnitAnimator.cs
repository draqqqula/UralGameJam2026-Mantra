using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();

        _unit.Health.OnDeath += PlayDown;
    }

    public void Play(UnitAnimation unitAnimation, out float duration)
    {
        var name = unitAnimation switch
        {
            UnitAnimation.Attack => "Attack",
            UnitAnimation.Damaged => "Damaged",
            UnitAnimation.Support => "Support",
            UnitAnimation.Down => "Down",
            UnitAnimation.Idle => "Idle",
            _ => "Idle",
        };

        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        duration = stateInfo.length;

        _animator.SetTrigger(name);
    }

    public void PlayDown()
    {
        Play(UnitAnimation.Down, out var _);
    }

    public Tween PlayFadeOutAnimation(float duration, AnimationCurve curve)
    {
        return _spriteRenderer.DOFade(0, duration).SetEase(curve);
    }

    private void OnDestroy()
    {
        _unit.Health.OnDeath -= PlayDown;
    }
}

public enum UnitAnimation
{
    Idle,
    Support,
    Damaged,
    Down,
    Attack
}
