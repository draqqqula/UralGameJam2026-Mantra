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
        SetMove(_canMove);
        _unit = GetComponent<Unit>();
    }

    public void SetMove(bool canMove)
    {
        _canMove = canMove;

        OnSetMove?.Invoke(_canMove);
    }
}