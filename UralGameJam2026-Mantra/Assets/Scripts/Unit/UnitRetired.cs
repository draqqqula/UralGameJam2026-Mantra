using UnityEngine;

public class UnitRetired : MonoBehaviour
{
    [SerializeField] private HighlightEffect _highlightEffect;
    private Unit _unit;

    private void Awake()
    {
        _unit = GetComponent<Unit>();

        _unit.Health.OnDeath += () => _highlightEffect.SetActive(true);
    }

    public void Resurrect()
    {
        _highlightEffect.SetActive(false);
    }
}
