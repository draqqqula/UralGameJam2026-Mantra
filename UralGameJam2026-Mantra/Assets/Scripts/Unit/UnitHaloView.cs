using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitHaloView : MonoBehaviour
{
    [SerializeField] private Image _halo;
    [SerializeField] private float _duration = .2f, _force = .8f, _magnitude = .2f;

    private Vector3 _originalScale;

    private float _current, _max = 1;

    private void Start()
    {
        _originalScale = _halo.transform.localScale;
    }

    private void Update()
    {
        _halo.transform.localScale = _originalScale * Mathf.Sin(Time.time * _force * _current) * _magnitude;
    }

    public void SetHalo(float current, float max)
    {
        _current = current;
        _max = max;
        float newFillAmount = _current / _max;
        _halo.DOFade(newFillAmount, _duration);
    }
}
