using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System.Collections;
using UnityEngine;

public class SelectionDisplay : MonoBehaviour
{
    [SerializeField] private SelectableUnit _selectable;
    [SerializeField] private SpriteRenderer _sprite;

    private void Start()
    {
        _selectable.State.Subscribe(HandleStateChanged).AddTo(this);
    }

    private void HandleStateChanged(SelectState state)
    {
        if (state == SelectState.Unavailable)
        {
            _sprite.color = new Color(0, 0, 0, 0);
        }
        else if (state == SelectState.Available)
        {
            _sprite.color = Color.white;
        }
        else if (state == SelectState.Hover)
        {
            _sprite.color = Color.yellow;
        }
        else if (state == SelectState.Selected)
        {
            _sprite.color = Color.red;
        }
    }
}