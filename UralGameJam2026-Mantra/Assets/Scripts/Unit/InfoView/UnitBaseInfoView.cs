using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBaseInfoView : MonoBehaviour
{
    [SerializeField] private GameObject _viewObject;
    [SerializeField] private Image _unitImage;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _damageText;
    [SerializeField] private TextMeshProUGUI _critChanceText;
    [SerializeField] private TextMeshProUGUI _critMultiText;
    [SerializeField] private TextMeshProUGUI _nameText;

    [SerializeField] private Camera _renderTextureCamera;
    [SerializeField] private Material _material;

    private TargetSystem _targetSystem;

    private void Start()
    {
        _unitImage.material = null;

        _targetSystem = TargetSystem.Instance;
        _targetSystem.OnSetTarget += DrawBaseInfo;
    }

    public void ShowView()
    {
        _viewObject.SetActive(true);
    }

    public void HideView()
    {
        _viewObject.SetActive(false);
    }

    private void DrawBaseInfo(Targetable target)
    {
        var unit = target.Unit;

        if (unit == null)
        {
            ResetInfo();

            return;
        }

        _healthText.text = $"{unit.Health.CurrentHealth}/{unit.Health.MaxHealth}";
        _defenseText.text = unit.Health.CurrentDefense.ToString();
        _damageText.text = $"{unit.Damage.MinDamage.ModValue}-{unit.Damage.MaxDamage.ModValue}";
        _critChanceText.text = $"{unit.Damage.CritChance.ModValue * 100}%";
        _critMultiText.text = $"{unit.Damage.CritMultiplyer.ModValue}x";
        _nameText.text = unit.UnitName;

        ChangeCameraPosition(unit.RenderCameraPoint);

        _unitImage.material = _material;
    }

    private void ResetInfo()
    {
        _unitImage.material = null;

        _healthText.text = $"0/0";
        _defenseText.text = "0";
        _damageText.text = $"0-0";
        _critChanceText.text = $"0%";
        _critMultiText.text = $"0x";
        _nameText.text = string.Empty;
    }

    private void ChangeCameraPosition(Transform point)
    {
        _renderTextureCamera.transform.localPosition = point.position;
    }

    private void OnDestroy()
    {
        _targetSystem.OnSetTarget -= DrawBaseInfo;
    }
}
