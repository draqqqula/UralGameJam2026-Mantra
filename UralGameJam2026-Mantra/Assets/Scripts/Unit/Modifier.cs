using UnityEngine;

[CreateAssetMenu(fileName = "Modifier_", menuName = "Unit/Modifier")]
public class Modifier : ScriptableObject
{
    public string ModifierName;
    public string ModifierDescription;
    public float Multiplyer;
    public int ModifierTurns;
}
