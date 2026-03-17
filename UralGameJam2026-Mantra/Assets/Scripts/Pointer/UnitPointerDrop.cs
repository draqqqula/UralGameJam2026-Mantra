using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPointerDrop : MonoBehaviour, IDropHandler
{
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag.TryGetComponent<Unit>(out var unit))
        {
            if (TestBattleManager.Instance.IsPlayerPartyMember(_unit))
            {
                unit.Use<SupportAction>(_unit);
                TestBattleManager.Instance.UpdateTurn();

                return;
            }

            unit.Use<AttackAction>(_unit);
            TestBattleManager.Instance.UpdateTurn();

        }
    }
}
