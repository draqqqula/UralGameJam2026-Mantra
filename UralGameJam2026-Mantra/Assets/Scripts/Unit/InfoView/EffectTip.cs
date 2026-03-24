using TMPro;
using UnityEngine;

public class EffectTip : MonoBehaviour
{
    public static EffectTip Instance;

    [SerializeField] private GameObject _tipObject;
    [SerializeField] private TextMeshProUGUI _effectTipText;
    [SerializeField] private RectTransform _tipTransform;

    [SerializeField] private float _inaccuracy = 0.2f;

    private Camera _camera;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            _camera = Camera.main;
        }
    }

    public void Show(string message)
    {
        _tipObject.SetActive(true);
        _effectTipText.text = message;
    }

    public void Hide()
    {
        _effectTipText.text = string.Empty;
        _tipObject.SetActive(false);
    }

    public void Move(Vector3 pos)
    {
        var mousePosition = _camera.WorldToScreenPoint(pos);

        var pivot = _tipTransform.pivot;

        if (mousePosition.x + _inaccuracy > 1f)
        {
            pivot.x = 1;
        }
        if(mousePosition.x - _inaccuracy < 0f)
        {
            pivot.x = 0;
        }
        if(mousePosition.y + _inaccuracy > 1f)
        {
            pivot.y = 0;
        }
        if(mousePosition.y - _inaccuracy < 0f)
        {
            pivot.y = 1;
        }

        _tipTransform.pivot = pivot;

        _tipObject.transform.position = pos;
    }
}