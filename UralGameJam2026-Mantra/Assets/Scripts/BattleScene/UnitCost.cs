using UnityEngine;

[CreateAssetMenu(fileName = "Unit_", menuName = "Unit/Unit")]
public class UnitCost : ScriptableObject
{
    public float Cost;
    public Unit Prefab;
}