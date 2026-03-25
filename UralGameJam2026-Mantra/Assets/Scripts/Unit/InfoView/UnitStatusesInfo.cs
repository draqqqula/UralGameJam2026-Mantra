using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitStatusesInfo : MonoBehaviour
{
    [SerializeField] private GameObject _statusFieldObject;
    [SerializeField] private Sprite _attackSprite, _critMultiSprite, _critChanceSprite;

    [SerializeField] private ModifierEffectInfo _prefab;

    private TargetSystem _targetSystem;

    private Unit _unit;

    private void Start()
    {
        _targetSystem = TargetSystem.Instance;
        _targetSystem.OnSetTarget += Show;
    }

    private void Show(Targetable target)
    {
        Hide();

        _unit = target.Unit;

        if (_unit == null)
        {
            return;
        }

        AddAttackSprite();
        AddCritChanceSprite();
        AddCritMultiSprite();
    }

    private void Hide()
    {
        foreach(Transform child in _statusFieldObject.transform)
        {
            if(child.TryGetComponent<ModifierEffectInfo>(out _))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void AddAttackSprite()
    {
        var setStats = new List<ModifierEffect>();

        if (_unit.Damage.MaxDamage.Modifiers.Any())
        {
            setStats.AddRange(_unit.Damage.MaxDamage.Modifiers);
        }
        if (_unit.Damage.MinDamage.Modifiers.Any())
        {
            setStats.AddRange(_unit.Damage.MinDamage.Modifiers);
        }

        var distinct = setStats.Distinct().ToList();
        if (!distinct.Any()) return;

        var instantiate = Instantiate(_prefab, _statusFieldObject.transform);
        instantiate.SetSprite(_attackSprite);
        instantiate.SetValue(distinct);
    }

    private void AddCritChanceSprite()
    {
        var setStats = new List<ModifierEffect>(_unit.Damage.CritChance.Modifiers);

        if (!setStats.Any()) return;

        var instantiate = Instantiate(_prefab, _statusFieldObject.transform);
        instantiate.SetSprite(_critChanceSprite);
        instantiate.SetValue(setStats);
    }

    private void AddCritMultiSprite()
    {
        var setStats = new List<ModifierEffect>(_unit.Damage.CritMultiplyer.Modifiers);

        if (!setStats.Any()) return;

        var instantiate = Instantiate(_prefab, _statusFieldObject.transform);
        instantiate.SetSprite(_critMultiSprite);

        instantiate.SetValue(setStats);
    }

    private void OnDestroy()
    {
        _targetSystem.OnSetTarget -= Show;
        Hide();
    }
}
