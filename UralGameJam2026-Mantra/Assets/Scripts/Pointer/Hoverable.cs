using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hoverable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;

    public void SetHover(bool value)
    {
        if (value)
        {
            _sprite.color = Color.yellow;
        }
        else
        {
            _sprite.color = Color.white;
        }
    }
}
