using System.Collections.Generic;
using UnityEngine;

public class ModifableValue
{
    private List<ModifierEffect> _modifiers = new();

    public float ModValue { get; private set; }
    private float _value;

    public ModifableValue(float value)
    {
        ModValue = _value = value;
    }

    public void ApplyModifier(Modifier modifier)
    {
        var effect = new ModifierEffect(modifier.ModifierTurns, modifier.Multiplyer);
        _modifiers.Add(effect);

        ModValue = _value;

        foreach(var mod in _modifiers)
        {
            ModValue *= mod.Multiplyer;
        }
    }

    public void CheckModifiers()
    {
        for(int i = 0; i < _modifiers.Count; i++)
        {
            _modifiers[i].Turn--;

            if (_modifiers[i].Turn <= 0) _modifiers.RemoveAt(i);
        }
    }
}

public class ModifierEffect
{
    public int Turn;
    public readonly float Multiplyer;

    public ModifierEffect(int turn, float multiplyer)
    {
        Turn = turn;
        Multiplyer = multiplyer;
    }
}
