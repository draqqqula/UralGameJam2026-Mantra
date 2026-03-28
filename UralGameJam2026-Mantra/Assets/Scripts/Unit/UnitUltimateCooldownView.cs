using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnitUltimateCooldownView : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private GameObject _viewObject, _ultaObject;
    [SerializeField] private Image _viewImage;
    [SerializeField] private float _duration = .2f, _force = .8f, _amplitude = 1f;

    private float _current, _max;
    private Vector3 _defaultRotation;

    private void Start()
    {
        _defaultRotation = _ultaObject.transform.eulerAngles;
        Hide();
    }

    private void Update()
    {
        if (_current + 1 == _max)
        {
            var newRotation = _defaultRotation;
            newRotation.z = Mathf.Sin(Time.time * _force) * _amplitude;

            _ultaObject.transform.eulerAngles = newRotation;
        }
        else
        {
            _ultaObject.transform.eulerAngles = _defaultRotation;
        }
    }

    public void UpdateView(float current, float max)
    {
        _viewObject.SetActive(true);

        _current = current;
        _max = max;

        float newAmount = (float)_current / (float)_max;

        _viewImage.color = _gradient.Evaluate(newAmount);
        _viewImage.DOFillAmount(newAmount, _duration);
    }

    public void Hide()
    {
        _viewObject.SetActive(false);
    }
}
