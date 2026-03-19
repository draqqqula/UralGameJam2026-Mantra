using R3;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Targetable : MonoBehaviour
{
    private ReactiveProperty<bool> _targeted = new ReactiveProperty<bool>();

    [field: SerializeField] public Unit Unit { get; private set; }
    public ReadOnlyReactiveProperty<bool> Targeted => _targeted;
    public bool IsTargetable
    {
        get
        {
            return Unit.IsAlive;
        }
    }

    private void Reset()
    {
        Unit = GetComponent<Unit>();
    }

    public void SetTargeted(bool value)
    {
        _targeted.Value = value;
    }
}