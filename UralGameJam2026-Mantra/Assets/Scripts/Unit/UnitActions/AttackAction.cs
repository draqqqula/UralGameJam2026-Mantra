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

        await UniTask.WaitForSeconds(Random.value, cancellationToken: token);

        _damageValue = _person.Damage.DealBaseDamage();
        _target.Health.ApplyDamage(_damageValue);

        print($"{_person.UnitName} attacks {_target.UnitName} with {_damageValue} damage!");

        _target.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
        _person.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _animDelay);

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);

        ActionHelper.EnableTargetSystem(cached);
    }

    public override void Plan(Unit person, Unit target)
    {
        _target = target;
        _person = person;
    }

    public override void Undo()
    {
        
    }
}
