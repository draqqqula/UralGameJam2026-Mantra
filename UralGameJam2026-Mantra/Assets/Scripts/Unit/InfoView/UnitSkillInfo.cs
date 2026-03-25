using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSkillInfo : MonoBehaviour
{
    [SerializeField] private GameObject _actionFieldObject;

    [SerializeField] private ActionEffectInfo _prefab;

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

        ShowSkill();
    }

    private void Hide()
    {
        foreach (Transform child in _actionFieldObject.transform)
        {
            if (child.TryGetComponent<ActionEffectInfo>(out _))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void ShowSkill()
    {
        foreach(var action in _unit.UnitActions)
        {
            var instantiate = Instantiate(_prefab, _actionFieldObject.transform);
            instantiate.SetValue(action);

            instantiate.SetText(action.ActionInfo.ActionName);
        }
    }

    private void OnDestroy()
    {
        _targetSystem.OnSetTarget -= Show;
        Hide();
    }
}
