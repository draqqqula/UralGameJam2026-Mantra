using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string UnitName;

    public UnitHealth Health;
    public UnitDamage Damage;

    public List<UnitAction> UnitActions = new();

    [SerializeField] private bool _isUniqueUnit = false;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Health.Setup();
        Damage.Setup();
    }

}
