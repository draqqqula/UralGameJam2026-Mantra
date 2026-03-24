using System.Collections;
using UnityEngine;

public class GlowHighlightEffect : HighlightEffect
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Material _outlineMaterial;
    [SerializeField] private Material _defaultMaterial;

    public override void SetActive(bool value)
    {
        _sprite.material = value ? _outlineMaterial : _defaultMaterial;
    }
}