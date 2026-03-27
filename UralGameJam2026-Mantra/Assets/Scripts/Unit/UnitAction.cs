using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class UnitAction : MonoBehaviour
{
    public UnitActionInfo ActionInfo;
    [SerializeField] protected Skill _skill;
    protected float _animDelay;
    protected Unit _person, _target;

    public abstract void Plan(Unit person, Unit target);
    public abstract UniTask Execute(CancellationToken token = default);
    public abstract bool CanUse();
}
