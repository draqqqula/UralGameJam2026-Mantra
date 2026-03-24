using System.Collections;
using UnityEngine;

public class ColorHighlightEffect : HighlightEffect
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Color _color;

    public override void SetActive(bool value)
    {
        if (value)
        {
            _sprite.color = _color;
        }
        else
        {
            _sprite.color = Color.white;
        }
    }
}