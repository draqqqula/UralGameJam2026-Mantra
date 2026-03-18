using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Hoverable _display;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("!!!");
        Debug.Log("!!!");
        _display.SetHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _display.SetHover(false);
    }
}