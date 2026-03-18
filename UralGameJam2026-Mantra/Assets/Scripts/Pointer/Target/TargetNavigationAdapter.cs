using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Targetable))]
public class TargetNavigationAdapter : MonoBehaviour
{
    [SerializeField] private Targetable _targetable;

    private void Reset()
    {
        _targetable = GetComponent<Targetable>();
    }

    private void Start()
    {
        TargetNavigation.Instance.Register(_targetable, transform.position.x);
    }
}