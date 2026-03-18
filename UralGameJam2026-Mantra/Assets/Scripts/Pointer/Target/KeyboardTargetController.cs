using System.Collections;
using UnityEngine;

public class KeyboardTargetController : MonoBehaviour
{
    [SerializeField] private TargetNavigation _navigation;
    [SerializeField] private KeyCode _left;
    [SerializeField] private KeyCode _right;
    [SerializeField] private KeyCode _submit;

    private void Update()
    {
        if (Input.GetKeyDown(_left))
        {
            if (_navigation.TryGetLeftNeighbour(TargetSystem.Instance.Current, out var left))
            {
                TargetSystem.Instance.TrySetTarget(left);
            }
        }

        if (Input.GetKeyDown(_right))
        {
            if (_navigation.TryGetRightNeighbour(TargetSystem.Instance.Current, out var right))
            {
                TargetSystem.Instance.TrySetTarget(right);
            }
        }

        if (Input.GetKeyDown(_submit))
        {
            TargetSystem.Instance.SubmitAction();
        }
    }
}