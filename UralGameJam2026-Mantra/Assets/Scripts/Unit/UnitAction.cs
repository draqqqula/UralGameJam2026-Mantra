using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    protected Unit _person, _target;

    public abstract void Invoke(Unit person, Unit target);
    public abstract void Undo();

}
