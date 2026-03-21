using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    protected float _animDelay;
    protected Unit _person, _target;

    public abstract void Plan(Unit person, Unit target);
    public abstract UniTask Execute(CancellationToken token);
    public abstract void Undo();
    public abstract bool CanUse();
}
