using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSkillInfo : MonoBehaviour
{
    [SerializeField] private GameObject _actionFieldObject;

    [SerializeField] private ActionEffectInfo _prefab;

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

    public void ShowSkill(Unit unit)
    {
        Hide();

        foreach (var action in unit.UnitActions)
        {
            var instantiate = Instantiate(_prefab, _actionFieldObject.transform);
            instantiate.SetValue(action);

            instantiate.SetText(action.ActionInfo.ActionName);
        }
    }

    private void OnDestroy()
    {
        Hide();
    }
}
