using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class SupportAction : UnitAction
{
    [SerializeField] private Skill _skill;

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

        //_target.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
        _person.GetComponent<UnitAnimator>().Play(UnitAnimation.Support, out _animDelay);

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);
        ActionHelper.EnableTargetSystem(cached);
    }

    public override void Plan(Unit person, Unit target)
    {
        _person = person;
        _target = target;

        if(_skill) _skill.Use(person, target);
    }

    public override void Undo()
    {
        if (_skill) _skill.Undo();
    }
}
