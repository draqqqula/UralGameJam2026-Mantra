using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class AttackAction : UnitAction
{
    private float _damageValue;

    public override bool CanUse()
    {
        return true;
    }

    public override async UniTask Execute(CancellationToken token)
    {
        var match = ServiceLocator.Instance.GetService<MatchManager>();
        var cached = match.CurrentMatchState;
        match.CurrentMatchState = MatchManager.State.Waiting;

        if (_target.RespondSkill(_person))
        {
            match.CurrentMatchState = cached;
            return;
        }

        if (_skill)
        {
            _skill.Use(_person, _target);
            await UniTask.WaitForSeconds(Random.value, cancellationToken: token);
            _animDelay = _skill.UseDelay();
        }
        else
        {
            _damageValue = _person.Damage.DealBaseDamage();
            _target.Health.ApplyDamage(_damageValue);

            print($"{_person.UnitName} attacks {_target.UnitName} with {_damageValue} damage!");

            _target.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
            _person.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _animDelay);
        }

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);
        match.CurrentMatchState = cached;
    }

    public override void Plan(Unit person, Unit target)
    {
        _target = target;
        _person = person;
    }
}
