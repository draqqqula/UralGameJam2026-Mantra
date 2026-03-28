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
        var cached = ActionHelper.DisableTargetSystem();

        if (_target.RespondSkill(_person))
        {
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

            _person.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _animDelay);
            if(_target.IsAlive)_target.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
        }

        var audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        audioManager.PlaySound(_person.UnitType + "Attack");
        await UniTask.Yield();

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);

        ActionHelper.EnableTargetSystem(cached);
    }

    public override void Plan(Unit person, Unit target)
    {
        _target = target;
        _person = person;
    }
}
