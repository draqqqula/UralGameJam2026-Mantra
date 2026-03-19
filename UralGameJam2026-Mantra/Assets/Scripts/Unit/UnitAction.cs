using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    protected Unit _person, _target;

    public abstract void Plan(Unit person, Unit target);
    public abstract void Execute();
    public abstract void Undo();
    public abstract bool CanUse();
}
