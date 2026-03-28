using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Targetable))]
public class PointerHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Targetable _targetable;

    private void Reset()
    {
        _targetable = GetComponent<Targetable>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            TargetSystem.Instance.SubmitAction();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TargetSystem.Instance.TrySetTarget(_targetable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TargetSystem.Instance.Current == _targetable)
        {
            TargetSystem.Instance.TrySetTarget(null);
        }
    }
}