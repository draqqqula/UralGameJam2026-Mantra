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

    public override void Invoke(Unit person, Unit target)
    {
        _target = target;
        _person = person;

        _person.Damage.MinDamage.ApplyModifier(_ultimateModifier);
        _person.Damage.MaxDamage.ApplyModifier(_ultimateModifier);

        _damageValue = _person.Damage.DealBaseDamage();

        _target.Health.ApplyDamage(_damageValue);

        print($"{_person.UnitName} attacks {_target.UnitName} with {_damageValue} damage!");

        _currentCooldown = _attackCooldown;
    }

    public override void Undo()
    {
        _currentCooldown = Mathf.Min(_currentCooldown + 1, _attackCooldown);
    }
}
