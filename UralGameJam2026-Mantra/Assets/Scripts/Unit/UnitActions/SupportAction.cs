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
        var cached = ActionHelper.DisableTargetSystem();

        var audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        audioManager.PlaySound(_person.UnitType + "Support");
        
        await UniTask.WaitForSeconds(Random.value, cancellationToken: token);
        print($"{_person.UnitName} helps {_target.UnitName} with smth");

        if (_skill)
        {
            _skill.Use(_person, _target);
            await UniTask.WaitForSeconds(Random.value, cancellationToken: token);

            _animDelay = _skill.UseDelay();
        }

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);
        ActionHelper.EnableTargetSystem(cached);
    }

    public override void Plan(Unit person, Unit target)
    {
        _person = person;
        _target = target;

    }
}
