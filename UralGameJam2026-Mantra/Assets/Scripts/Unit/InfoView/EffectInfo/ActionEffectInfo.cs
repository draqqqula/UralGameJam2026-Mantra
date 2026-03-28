using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionEffectInfo : EffectInfo<UnitAction>
{
    [SerializeField] private TextMeshProUGUI _actionName;
    [SerializeField] private Image _image;

    public void SetValue(UnitAction value)
    {
        Value = value;
    }

    public void SetText(string text)
    {
        _actionName.text = text;
    }

    public void SetIcon(Sprite icon)
    {
        _image.sprite = icon;
    }

    public override string Describe()
    {
        if(!Value) return string.Empty;

        var info = Value.ActionInfo.ActionDescription;
        return info;
    }
}