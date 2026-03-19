using UnityEngine;

public abstract class Skill : ScriptableObject
{
    public abstract void Use(Unit person, Unit target);
    public abstract void Undo();
}
