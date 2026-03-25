using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System.Collections;
using UnityEngine;

public class SelectionDisplay : MonoBehaviour
{
    [SerializeField] private SelectableUnit _selectable;
    [SerializeField] private SpriteRenderer _sprite;
    private Tween _cached;

    private void Start()
    {
        _selectable.State.Subscribe(HandleStateChanged).AddTo(this);
    }

    private void HandleStateChanged(SelectState state)
    {
        _cached?.Kill();

        if (state == SelectState.Unavailable)
        {
            _cached = _sprite.DOFade(0, 0.4f).SetLink(gameObject);
        }
        else if (state == SelectState.Available)
        {
            _cached = _sprite.DOColor(Color.white, 0.4f).SetLink(gameObject);
        }
        else if (state == SelectState.Hover)
        {
            _cached = _sprite.DOColor(Color.yellow, 0.4f).SetLink(gameObject);
        }
        else if (state == SelectState.Selected)
        {
            _cached = _sprite.DOColor(Color.red, 0.4f).SetLink(gameObject);
        }
    }
}