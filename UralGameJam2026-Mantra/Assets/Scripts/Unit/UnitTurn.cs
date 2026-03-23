using UnityEngine;

[RequireComponent(typeof(Unit))]
public class UnitTurn : MonoBehaviour
{
    public bool CanMove => _canMove;

    public event System.Action<bool> OnSetMove;

    private bool _canMove = true;

    private Unit _unit;

    private void Start()
    {
        _unit = GetComponent<Unit>();
        SetMove(_canMove);
    }

    public void SetMove(bool canMove)
    {
        _canMove = canMove && _unit.IsAlive;

        OnSetMove?.Invoke(_canMove);
    }
}