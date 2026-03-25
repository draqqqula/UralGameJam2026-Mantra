using R3;
using System.Collections;
using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    private ReactiveProperty<SelectState> _state = new ReactiveProperty<SelectState>(SelectState.Unavailable);
    [SerializeField] private Unit _unit;
    [SerializeField] private UnitTurn _turn;
    public ReadOnlyReactiveProperty<SelectState> State => _state;

    private void Update()
    {
        _state.Value = GetState();
    }

    private SelectState GetState()
    {
        var battle = ServiceLocator.Instance.GetService<BattleManager>();
        var currentUnit = battle.GetCurrentUnit();

        var isInitiator = _unit == currentUnit;
        var isTargeted = TargetSystem.Instance.Current != null && TargetSystem.Instance.Current.Unit == _unit;
        var noneSelected = currentUnit == null;
        var canMove = _turn.CanMove;

        if (canMove && _unit.ShouldShowAura)
        {
            if (isInitiator)
            {
                return SelectState.Selected;
            }
            else if (isTargeted && noneSelected)
            {
                return SelectState.Hover;
            }
            else
            {
                return SelectState.Available;
            }
        }
        else
        {
            return SelectState.Unavailable;
        }
    }
}