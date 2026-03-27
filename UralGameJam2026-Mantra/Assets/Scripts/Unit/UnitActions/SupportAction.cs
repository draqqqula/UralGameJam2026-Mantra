using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SupportAction : UnitAction
{
    public override bool CanUse()
    {
        return true;
    }

    public override async UniTask Execute(CancellationToken token)
    {
        await UniTask.WaitForSeconds(Random.value, cancellationToken: token);
        print($"{_person.UnitName} helps {_target.UnitName} with smth");

        if (_skill) _skill.Use(_person, _target);

        _person.GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out _animDelay);

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);
    }

    public override void Plan(Unit person, Unit target)
    {
        _person = person;
        _target = target;

    }
}
