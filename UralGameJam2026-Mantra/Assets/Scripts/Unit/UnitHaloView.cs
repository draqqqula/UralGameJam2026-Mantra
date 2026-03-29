using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitHaloView : MonoBehaviour
{
    [SerializeField] private Image _halo;
    [SerializeField] private float _duration = .2f, _force = .8f, _magnitude = .2f;

    private Unit _unit;

    private Vector3 _originalScale;
    private Transform _posPoint;

    private float _current, _max = 1;

    public void Init(Transform posPoint)
    {
        _originalScale = _halo.transform.localScale;
        _posPoint = posPoint;
    }

    private void Update()
    {
        _halo.transform.localScale = _originalScale * Mathf.Sin(Time.time * _force * _current) * _magnitude;
        transform.position = _posPoint.position;
    }

    public void SetHalo(float current, float max)
    {
        _current = current;
        _max = max;
        float newFillAmount = _current / _max;
        _halo.DOFade(newFillAmount, _duration);
    }

    private void Hide()
    {
        _halo.gameObject.SetActive(false);
    }

    private void Show()
    {
        _halo.gameObject.SetActive(true);
    }

    public void Init(Unit unit)
    {
        _unit = unit;

        _unit.Health.OnDeath += Hide;
        _unit.Health.OnResurrect += Show;
    }

    private void OnDestroy()
    {
        _unit.Health.OnDeath -= Hide;
        _unit.Health.OnResurrect -= Show;
    }
}
