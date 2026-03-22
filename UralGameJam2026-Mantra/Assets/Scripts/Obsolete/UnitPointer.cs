using UnityEngine;
using UnityEngine.EventSystems;

public class UnitPointer : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private bool _isDragging = false;
    private Vector2 _defaultPosition;

    private Unit _unit;
    private Camera _camera;
    private BoxCollider2D _boxCollider;

    private void Start()
    {
        _defaultPosition = transform.localPosition;
        _camera = Camera.main;
        _boxCollider = GetComponent<BoxCollider2D>();
        _unit = GetComponent<Unit>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!TestBattleManager.Instance.UnitIsCurrent(_unit))
        {
            return;
        }
        if (!_isDragging) return;

        var newPosition = _camera.ScreenToWorldPoint(eventData.position);
        newPosition.z = 0;
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        transform.position = _defaultPosition;
        _boxCollider.enabled = true;
        _isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TestBattleManager.Instance.UnitIsCurrent(_unit))
        {
            return;
        }

        _boxCollider.enabled = false;
        _isDragging = true;
    }
}
