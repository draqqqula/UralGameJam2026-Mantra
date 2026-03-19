using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Targetable : MonoBehaviour
{
    [field: SerializeField] public Unit Unit { get; private set; }
    public bool Targeted { get; private set; } = false;
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
        Targeted = value;
    }
}