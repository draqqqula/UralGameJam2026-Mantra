using UnityEngine;

public class SupportAction : UnitAction
{
    [SerializeField] private Skill _skill;

    public override bool CanUse()
    {
        return true;
    }

    public override void Invoke(Unit person, Unit target)
    {
        _person = person;
        _target = target;

        print($"{_person.UnitName} helps {_target.UnitName} with smth");
    }

    public override void Undo()
    {
        throw new System.NotImplementedException();
    }
}
