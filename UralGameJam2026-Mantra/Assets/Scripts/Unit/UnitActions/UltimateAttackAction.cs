using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class UltimateAttackAction : UnitAction
{
    [SerializeField] private int _attackCooldown;
    [SerializeField] private Modifier _ultimateModifier;

    private float _damageValue;
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

        var audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        audioManager.PlaySound("Ulta");
        await UniTask.WaitForSeconds(Random.value, cancellationToken: token);

        if (_ultimateModifier)
        {
            _person.Damage.MinDamage.ApplyModifier(_ultimateModifier);
            _person.Damage.MaxDamage.ApplyModifier(_ultimateModifier);
        }

        _damageValue = _person.Damage.DealBaseDamage();

        if (!_target) _target = ServiceLocator.Instance.GetService<BattleManager>().GetRandomEnemy();
        _target.Health.ApplyDamage(_damageValue);

        print($"{_person.UnitName} attacks {_target.UnitName} with {_damageValue} damage!");

        _currentCooldown = _attackCooldown;

        _target.GetComponent<UnitAnimator>().Play(UnitAnimation.Damaged, out _);
        _person.GetComponent<UnitAnimator>().Play(UnitAnimation.Attack, out _animDelay);

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);

    }

    public override void Plan(Unit person, Unit target)
    {
        _target = target;
        _person = person;
    }

    public override void Undo()
    {
        _currentCooldown = Mathf.Min(_currentCooldown + 1, _attackCooldown);
    }
}
