using UnityEngine;

public class AttackAction : UnitAction
{
    private float _damageValue;

    public override bool CanUse()
    {
        return true;
    }

    public override void Execute()
    {
        _damageValue = _person.Damage.DealBaseDamage();

        _target.Health.ApplyDamage(_damageValue);

        print($"{_person.UnitName} attacks {_target.UnitName} with {_damageValue} damage!");
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
