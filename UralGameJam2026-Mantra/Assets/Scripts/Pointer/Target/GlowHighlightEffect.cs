using System.Collections;
using UnityEngine;

public class GlowHighlightEffect : HighlightEffect
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private string _outlineMaterialName;
    [SerializeField] private Material _defaultMaterial;
    private Material _outlineMaterial;

    private void Start()
    {
        _outlineMaterial = MaterialRepository.Instance.MaterialByKey[_outlineMaterialName];
    }

    public override void SetActive(bool value)
    {
        if (_outlineMaterial == null)
        {
            return;
        }
        _sprite.material = value ? _outlineMaterial : _defaultMaterial;
    }
}