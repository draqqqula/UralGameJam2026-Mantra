using UnityEngine;
using UnityEngine.EventSystems;

public abstract class EffectInfo<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] protected T Value;
    protected EffectTip _tip;

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _tip = EffectTip.Instance;
    }

    public abstract string Describe();


    public void OnPointerEnter(PointerEventData eventData)
    {
        _tip.Show(Describe());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _tip.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        _tip.Move(Input.mousePosition);
    }
}
