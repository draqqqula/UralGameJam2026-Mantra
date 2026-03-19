using UnityEngine;

public class SupportAction : UnitAction
{
    [SerializeField] private Skill _skill;

    public override bool CanUse()
    {
        return true;
    }

    public override void Execute()
    {
        print($"{_person.UnitName} helps {_target.UnitName} with smth");
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
