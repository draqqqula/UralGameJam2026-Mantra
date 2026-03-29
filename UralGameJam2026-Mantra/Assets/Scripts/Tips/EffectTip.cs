using TMPro;
using UnityEngine;

public class EffectTip : MonoBehaviour
{
    public static EffectTip Instance;

    [SerializeField] private GameObject _tipObject;
    [SerializeField] private TextMeshProUGUI _effectTipText;
    [SerializeField] private RectTransform _tipTransform;

    [SerializeField] private Border _border;
    [SerializeField] private PivotPosition _pivotPosition;

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
        if (string.IsNullOrEmpty(message)) return;

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
        var mousePosition = _camera.WorldToViewportPoint(pos).normalized;

        var pivot = _tipTransform.pivot;
        if (mousePosition.x + _border.RightBorder > 1f)
        {
            pivot.x = _pivotPosition.PositiveX;
        }
        if(mousePosition.x - _border.LeftBorder < 0f)
        {
            pivot.x = _pivotPosition.NegativeX;
        }
        if(mousePosition.y + _border.TopBorder > 1f)
        {
            pivot.y = _pivotPosition.PositiveY;
        }
        if(mousePosition.y - _border.BottomBorder < 0f)
        {
            pivot.y = _pivotPosition.NegativeY;
        }

        _tipTransform.pivot = pivot;

        _tipObject.transform.position = pos;
    }
}

[System.Serializable]
public class PivotPosition
{
    public float PositiveX = 1, NegativeX = 0;
    public float PositiveY = 1, NegativeY = 0;
}

[System.Serializable] 
public class Border
{
    public float LeftBorder = 1, RightBorder = .5f, TopBorder = 1, BottomBorder = .5f;
}