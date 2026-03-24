using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoView : MonoBehaviour
{
    [SerializeField] private GameObject _viewObject, _statusesObject;
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
        _targetSystem = TargetSystem.Instance;

        _targetSystem.OnSetTarget += DrawInfo;
    }

    private void DrawInfo(Targetable target)
    {
        var unit = target.Unit;

        if (unit == null)
        {
            _unitImage.material = null;

            return;
        }

        _healthText.text = $"{unit.Health.CurrentHealth}/{unit.Health.MaxHealth}";
        _defenseText.text = unit.Health.CurrentDefense.ToString();
        _damageText.text = $"{unit.Damage.MinDamage.ModValue}-{unit.Damage.MaxDamage.ModValue}";
        _critChanceText.text = $"{unit.Damage.CritChance.ModValue * 100}%";
        _critMultiText.text = $"{unit.Damage.CritMultiplyer.ModValue * 100}x";
        _nameText.text = unit.UnitName;

        ChangeCameraPosition(unit.RenderCameraPoint);

        _unitImage.material = _material;
    }

    private void ChangeCameraPosition(Transform point)
    {
        _renderTextureCamera.transform.localPosition = point.position;
    }

    private void OnDestroy()
    {
        _targetSystem.OnSetTarget -= DrawInfo;
    }
}
