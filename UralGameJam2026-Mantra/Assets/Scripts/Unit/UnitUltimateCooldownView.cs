using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnitUltimateCooldownView : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private GameObject _viewObject;
    [SerializeField] private Image _viewImage;
    [SerializeField] private float _duration = .2f;

    private float _current, _max;

    private void Start()
    {
        Hide();
    }

    public void UpdateView(float current, float max)
    {
        _viewObject.SetActive(true);

        _current = current;
        _max = max;

        float newAmount = _current / _max;

        _viewImage.color = _gradient.Evaluate(newAmount);
        _viewImage.DOFillAmount(newAmount, _duration);
    }

    public void Hide()
    {
        _viewObject.SetActive(false);
    }
}
