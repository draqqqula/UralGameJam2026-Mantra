using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;

public class HaloEffect : MonoBehaviour
{
    [SerializeField] private SelectableUnit _unit;
    [SerializeField] private SpriteRenderer _sprite;

    private void Start()
    {
        _unit.State.Subscribe(HandleStateChanged).AddTo(this);
    }

    private void HandleStateChanged(SelectState state)
    {
        if (state == SelectState.Selected)
        {
            _sprite.color = Color.white;
        }
        else
        {
            _sprite.color = new Color(0, 0, 0, 0);
        }
    }
}
