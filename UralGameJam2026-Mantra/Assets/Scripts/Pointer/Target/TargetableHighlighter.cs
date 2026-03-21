using R3;
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

    private void Awake()
    {
        _targetable.Targeted.Subscribe(HandleStateChanged).AddTo(this);
    }

    private void HandleStateChanged(bool targetable)
    {
        _selfEffect.SetActive(false);
        _attackEffect.SetActive(false);
        _supportEffect.SetActive(false);

        if (targetable)
        {
            var effect = GetEffect();

            if(effect) effect.SetActive(true);
        }
    }

    private HighlightEffect GetEffect()
    {
        var relationship = TestBattleManager.Instance.GetRelationShipToCurrent(_targetable.Unit);

        if(TestBattleManager.Instance.IsEnemyTurn) return null;

        switch (relationship)
        {
            case UnitRelationship.Enemy: return _attackEffect;
            case UnitRelationship.Friend: return _supportEffect;
            default: return _selfEffect;
        }
    }
}