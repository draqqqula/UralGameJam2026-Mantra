using UnityEngine;

public class AttackAction : UnitAction
{
    private float _damageValue;

    public override void Invoke(Unit person, Unit target)
    {
        //пока что просто бьет
        _target = target;
        _person = person;
        _damageValue = _person.Damage.DealBaseDamage();

        _target.Health.ApplyDamage(_damageValue);

        print($"{_person.UnitName} attacks {_target.UnitName} with {_damageValue} damage!");
    }

    public override void Undo()
    {
        
    }
}
