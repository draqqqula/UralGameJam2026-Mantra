using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public abstract void Use(params Unit[] units);
    public abstract float UseDelay();
}
