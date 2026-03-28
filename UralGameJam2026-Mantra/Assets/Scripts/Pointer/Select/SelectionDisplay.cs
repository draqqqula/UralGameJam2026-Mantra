using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System.Collections;
using UnityEngine;

public class SelectionDisplay : MonoBehaviour
{
    [SerializeField] private SelectableUnit _selectable;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private SpriteRenderer _glow;
    [SerializeField] private Unit _unit;
    [SerializeField] private Color _red;
    [SerializeField] private Color _blue;
    [SerializeField] private Color _yellow;
    [SerializeField] private Sprite _selected;
    [SerializeField] private Sprite _default;
    private Tween _cachedA;
    private Tween _cachedB;

    private void Start()
    {
        _unit = GetComponentInParent<Unit>();
        _selectable.State.Subscribe(HandleStateChanged).AddTo(this);
    }

    private void HandleStateChanged(SelectState state)
    {
        _cachedA?.Kill();
        _cachedB?.Kill();

        var isFriend = ServiceLocator.Instance.GetService<BattleManager>().IsPlayerPartyMember(_unit);
        var defaultColor = isFriend ? _blue : _red;

        if (state == SelectState.Unavailable)
        {
            _cachedA = _sprite.DOFade(0, 0.4f).SetLink(gameObject);
            _cachedB = _glow.DOFade(0, 0.4f).SetLink(gameObject);
        }
        else if (state == SelectState.Available)
        {
            _cachedA = _sprite.DOFade(1, 0.4f).SetLink(gameObject);
            _cachedB = _glow.DOColor(defaultColor, 0.4f).SetLink(gameObject);
        }
        else if (state == SelectState.Hover)
        {
            _cachedA = _sprite.DOFade(1, 0.4f).SetLink(gameObject);
            _cachedB = _glow.DOColor(_yellow, 0.4f).SetLink(gameObject);
        }
        else if (state == SelectState.Selected)
        {
            _cachedA = _sprite.DOFade(1, 0.4f).SetLink(gameObject);
            _cachedB = _glow.DOColor(Color.white, 0.4f).SetLink(gameObject);
        }
    }
}