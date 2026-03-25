using TMPro;
using UnityEngine;

public class ActionEffectInfo : EffectInfo<UnitAction>
{
    [SerializeField] private TextMeshProUGUI _actionName;

    public void SetValue(UnitAction value)
    {
        Value = value;
    }

    public void SetText(string text)
    {
        _actionName.text = text;
    }

    public override string Describe()
    {
        if(!Value) return string.Empty;

        var info = Value.ActionInfo.ActionDescription;
        return info;
    }
}