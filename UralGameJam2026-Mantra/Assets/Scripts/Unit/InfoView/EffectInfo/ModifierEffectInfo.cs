using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModifierEffectInfo : EffectInfo<List<ModifierEffect>>
{
    [SerializeField] private Image _effectIcon;

    public void SetValue(List<ModifierEffect> value)
    {
        Value = value;
    }

    public void SetSprite(Sprite sprite)
    {
        _effectIcon.sprite = sprite;
    }

    public override string Describe()
    {
        if (!Value.Any()) return string.Empty;

        var info = string.Join('\n', Value
            .Select(x => string
            .Join('\n', x.Name, x.Description, $"{x.Turn} ходов осталось")));
        return info;
    }
}