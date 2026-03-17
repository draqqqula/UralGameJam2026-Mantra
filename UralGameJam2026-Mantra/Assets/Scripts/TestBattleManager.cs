using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestBattleManager : MonoBehaviour
{
    public static TestBattleManager Instance;

    [SerializeField] private Party _playersUnits;
    [SerializeField] private Party _enemiesUnits;

    private Queue<Unit> _unitOrder = new();

    private Unit _currentUnit;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Setup();
    }

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
        foreach (var unit in _playersUnits.Members)
        {
            if (!unit.IsAlive) continue;
            _unitOrder.Enqueue(unit);
        }

        foreach(var unit in _enemiesUnits.Members)
        {
            if (!unit.IsAlive) continue;
            _unitOrder.Enqueue(unit);
        }

        UpdateTurn();
    }

    public void UpdateTurn()
    {
        if (!_unitOrder.TryDequeue(out _currentUnit))
        {
            Setup();
            return;
        }

        foreach (var unit in _unitOrder)
        {
            if(!unit.IsAlive)
            {
                unit.gameObject.SetActive(false);
                continue;
            }

            //modifiers need to be check in another place, but for now it here
            unit.Damage.CritMultiplyer.CheckModifiers();
            unit.Damage.CritChance.CheckModifiers();

            unit.Damage.MaxDamage.CheckModifiers();
            unit.Damage.MinDamage.CheckModifiers();
        }

        print($"Now {_currentUnit.UnitName} turn");

        if(!_playersUnits.Members.Contains(_currentUnit))
        {
            var action = _currentUnit.UnitActions[Random.Range(0, _currentUnit.UnitActions.Count)];
            var target = _playersUnits.Members[Random.Range(0, _playersUnits.Members.Count)];
            action.Invoke(_currentUnit, target);
            UpdateTurn();
            return;
        }
    }

    public bool UnitIsCurrent(Unit unit)
    {
        return _currentUnit == unit;
    }

    public bool IsPlayerPartyMember(Unit target)
    {
        return _playersUnits.Members.Contains(target);
    }

    public bool IsEnemyPartyMember(Unit target)
    {
        return _enemiesUnits.Members.Contains(target);
    }
}
