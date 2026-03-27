using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class UltimateAttackAction : UnitAction
{
    [SerializeField] private int _attackCooldown;

    private int _currentCooldown;

    public override bool CanUse()
    {
        return _attackCooldown == 0;
    }

    public void DecreaseCooldown()
    {
        if(_currentCooldown > 0)
        {
            _currentCooldown--;
        }
    }

    public override async UniTask Execute(CancellationToken token = default)
    {
        if(_person == null)
        {
            _person = GetComponentInParent<Unit>();
        }


        if (_skill)
        {
            print($"{_person.UnitName} do ultimate skill on {_target.UnitName}");
            _skill.Use(_target);

            await UniTask.WaitForSeconds(Random.value, cancellationToken: token);

            _animDelay = _skill.UseDelay();
        }

        _currentCooldown = _attackCooldown;

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);
    }

    public override void Plan(Unit person, Unit target)
    {
        _target = target;
        _person = person;
    }
}
