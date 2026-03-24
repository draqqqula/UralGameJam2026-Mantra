using System.Collections;
using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _defaultValue;
    [SerializeField] private float _amplitude;
    [SerializeField] private float _speed = 1f;

    void Update()
    {
        _material.SetFloat("_OutlineRadius", _defaultValue + _amplitude * _curve.Evaluate(Time.time * _speed));
    }
}