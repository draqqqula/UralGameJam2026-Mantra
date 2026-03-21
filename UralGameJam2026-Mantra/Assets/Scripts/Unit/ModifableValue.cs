using System.Collections.Generic;
using UnityEngine;

public class ModifableValue
{
    public List<ModifierEffect> Modifiers { get; private set; }

    public float ModValue { get; private set; }
    private float _value;

    public ModifableValue(float value)
    {
        Modifiers = new();
        ModValue = _value = value;
    }

    public void ApplyModifier(Modifier modifier)
    {
        var effect = new ModifierEffect(modifier.ModifierTurns, modifier.Multiplyer);
        Modifiers.Add(effect);

        ModValue = _value;

        foreach(var mod in Modifiers)
        {
            ModValue *= mod.Multiplyer;
        }
    }

    public void CheckModifiers()
    {
        for(int i = 0; i < Modifiers.Count; i++)
        {
            Modifiers[i].Turn--;

            if (Modifiers[i].Turn <= 0) Modifiers.RemoveAt(i);
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
