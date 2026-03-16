using System.Collections.Generic;
using UnityEngine;

public class TestBattleManager : MonoBehaviour
{
    public Unit CurrentUnit => _currentUnit;

    [SerializeField] private List<Unit> _playersUnits = new();
    [SerializeField] private List<Unit> _enemiesUnits = new();

    private Queue<Unit> _unitOrder = new();

    private Unit _currentUnit;

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.S))
        {
            UpdateTurn();
        }
#endif
    }

    public void Setup()
    {
        foreach (var unit in _playersUnits)
        {
            _unitOrder.Enqueue(unit);
        }

        foreach(var unit in _enemiesUnits)
        {
            _unitOrder.Enqueue(unit);
        }
    }

    public void UpdateTurn()
    {
        if (!_unitOrder.TryDequeue(out _currentUnit))
        {
            Setup();
            return;
        }

        foreach(var unit in _unitOrder)
        {
            //modifiers need to be check in another place, but for now it here
            unit.Damage.CritMultiplyer.CheckModifiers();
            unit.Damage.CritChance.CheckModifiers();

            unit.Damage.MaxDamage.CheckModifiers();
            unit.Damage.MinDamage.CheckModifiers();
        }

        print($"Now {_currentUnit.UnitName} turn");

        if(!_playersUnits.Contains(_currentUnit))
        {
            var action = _currentUnit.UnitActions[Random.Range(0, _currentUnit.UnitActions.Count)];
            var target = _playersUnits[Random.Range(0, _playersUnits.Count)];
            action.Invoke(_currentUnit, target);
            UpdateTurn();
            return;
        }
    }
}
