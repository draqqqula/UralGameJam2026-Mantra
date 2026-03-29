using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitHaloView : MonoBehaviour
{
    [SerializeField] private Image _halo;
    [SerializeField] private float _force = .8f, _magnitude = .2f;

    private Unit _unit;

    private Vector3 _originalScale;
    private Transform _posPoint;

    private float _current, _max = 1;

    private void Start()
    {
        Hide();
    }

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

        var min05 = Mathf.Min(_current + 0.5f, _max);
        var min1 = Mathf.Min(_current + 1f, _max);

        if (min05 == _max || min1 == _max)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Hide()
    {
        _halo.gameObject.SetActive(false);
    }

    private void Show()
    {
        var min05 = Mathf.Min(_current + 0.5f, _max);
        var min1 = Mathf.Min(_current + 1f, _max);
        if (min05 == _max || min1 == _max)
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
