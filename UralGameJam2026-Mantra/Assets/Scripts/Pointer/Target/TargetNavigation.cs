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
        var index = currentIndex + offset;
        if (index < 0 || index >= _ordered.Count)
        {
            target = null;
            return false;
        }
        target = _ordered[index].Item1;
        return target;
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