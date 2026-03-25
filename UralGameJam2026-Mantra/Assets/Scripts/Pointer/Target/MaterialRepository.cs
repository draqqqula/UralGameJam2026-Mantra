using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialRepository : MonoBehaviour
{
    public static MaterialRepository Instance;

    [Serializable]
    public class MaterialAndKey
    {
        public string key; 
        public Material value;
    }

    [SerializeField] private MaterialAndKey[] _materials;
    private Dictionary<string, Material> _materialByKey;

    public IReadOnlyDictionary<string, Material> MaterialByKey => _materialByKey;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _materialByKey = _materials.ToDictionary(it => it.key, it => new Material(it.value));
    }   
}