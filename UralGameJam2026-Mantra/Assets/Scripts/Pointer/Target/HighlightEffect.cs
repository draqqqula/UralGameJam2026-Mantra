using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Color _color;

    public void SetActive(bool value)
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
