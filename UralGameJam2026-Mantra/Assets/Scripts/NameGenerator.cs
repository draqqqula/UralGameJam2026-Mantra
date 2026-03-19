using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{
    public static NameGenerator Instance;

    [SerializeField] private List<string> _names = new();
    [SerializeField] private List<string> _dignity = new();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public string GenerateName()
    {
        if (!_names.Any() || !_dignity.Any()) return "???";

        var name = $"{_names[Random.Range(0, _names.Count)]}, {_dignity[Random.Range(0, _dignity.Count)]}";
        return name;
    }
}
