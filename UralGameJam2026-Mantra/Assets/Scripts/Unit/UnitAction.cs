using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    [SerializeField] protected UnitAnimation _animName;

    protected float _animDelay;
    protected Unit _person, _target;

    public abstract void Plan(Unit person, Unit target);
    public abstract UniTask Execute();
    public abstract void Undo();
    public abstract bool CanUse();
}
