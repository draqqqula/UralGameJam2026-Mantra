using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;


public class TargetNavigation : MonoBehaviour
{
    public static TargetNavigation Instance;

    private List<(Targetable, float)> _ordered = new ();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool TryGetLeftNeighbour(Targetable current, out Targetable left)
    {
        return TryGetOffset(current, -1, out left);
    }

    public bool TryGetRightNeighbour(Targetable current, out Targetable right)
    {
        return TryGetOffset(current, 1, out right);
    }

    private bool TryGetOffset(Targetable current, int offset, out Targetable target)
    {
        var currentIndex = _ordered.FindIndex(it => it.Item1 == current);
        var index = Mathf.Clamp(currentIndex + offset, 0, _ordered.Count - 1);

        target = _ordered[index].Item1;

        if(!target.Unit.IsAlive)
        {
            int additionalOffset = offset + Sign(offset);
            var res = TryGetOffset(current, additionalOffset, out target);
            return res;
        }
        return target;
    }

    private int Sign(int value)
    {
        return value <= 0 ? -1 : 1;
    }

    public void Register(Targetable item, float x)
    {
        for (int i = 0; i < _ordered.Count; i++)
        {
            if (_ordered[i].Item2 >= x)
            {
                _ordered.Insert(i, (item, x));
                return;
            }
        }
        _ordered.Add((item, x));
    }
}