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
            var battle = ServiceLocator.Instance.GetService<BattleManager>();
            if (battle != null && battle.IsSelecting())
            {
                _selfEffect.SetActive(true);
                return;
            }

            var effect = GetEffect();

            if(effect) effect.SetActive(true);
        }
    }

    private HighlightEffect GetEffect()
    {
        var battleManager = ServiceLocator.Instance.GetService<BattleManager>();
        var matchManager = ServiceLocator.Instance.GetService<MatchManager>();

        var relationship = battleManager.GetRelationShipToCurrent(_targetable.Unit);

        if(battleManager.IsEnemyTurn()) return null;
        if (matchManager.CurrentMatchState == MatchManager.State.Recrouting) return _selfEffect;
        
        switch (relationship)
        {
            case UnitRelationship.Enemy: return _attackEffect;
            case UnitRelationship.Friend: return _supportEffect;
            case UnitRelationship.None: return null;
            default: return _selfEffect;
        }
    }
}