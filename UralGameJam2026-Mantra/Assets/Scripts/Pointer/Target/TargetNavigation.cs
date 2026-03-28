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
        target = null;

        if (_ordered.Count == 0)
            return false;

        int index = _ordered.FindIndex(t => t.Item1 == current);
        if (index == -1)
            return false;

        int direction = Sign(offset);
        int i = index + direction;

        while (i >= 0 && i < _ordered.Count)
        {
            var candidate = _ordered[i].Item1;

            if (TargetSystem.Instance.IsTargetable(candidate))
            {
                target = candidate;
                return true;
            }

            i += direction;
        }

        return false;
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