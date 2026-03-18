using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Targetable))]
public class TargetableHighlighter : MonoBehaviour
{
    [SerializeField] private Targetable _targetable;
    [SerializeField] private HighlightEffect _selfEffect;
    [SerializeField] private HighlightEffect _attackEffect;
    [SerializeField] private HighlightEffect _supportEffect;

    private void Reset()
    {
        _targetable = GetComponent<Targetable>();
    }

    private void Update()
    {
        // inefficient, change to R3
        _selfEffect.SetActive(false);
        _attackEffect.SetActive(false);
        _supportEffect.SetActive(false);

        if (_targetable.Targeted)
        {
            var effect = GetEffect();

            effect.SetActive(true);
        }
    }


    private HighlightEffect GetEffect()
    {
        var relationship = TestBattleManager.Instance.GetRelationShipToCurrent(_targetable.Unit);
        switch (relationship)
        {
            case UnitRelationship.Self: return _selfEffect;
            case UnitRelationship.Enemy: return _attackEffect;
            case UnitRelationship.Friend: return _supportEffect;
        }
        return _selfEffect;
    }
}