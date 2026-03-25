using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitBaseInfoView : MonoBehaviour, IService
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
    private Unit _unit;

    private (float, float) _damage;
    private (float, float) _health;
    private (float, float) _crit;

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
        UpdateUnit(ref _unit, target.Unit);

        if (_unit == null)
        {
            ResetInfo();

            return;
        }

        _damage.Item1 = _unit.Damage.MinDamage.ModValue;
        _damage.Item2 = _unit.Damage.MaxDamage.ModValue;

        _health.Item1 = _unit.Health.MaxHealth;
        _health.Item2 = _unit.Health.CurrentHealth;

        _crit.Item1 = _unit.Damage.CritChance.ModValue;
        _crit.Item2 = _unit.Damage.CritMultiplyer.ModValue;

        _healthText.text = $"{_health.Item2}/{_health.Item1}";
        _defenseText.text = _unit.Health.CurrentDefense.ToString();
        _damageText.text = $"{_damage.Item1}-{_damage.Item2}";
        _critChanceText.text = $"{_crit.Item1 * 100}%";
        _critMultiText.text = $"{_crit.Item2}x";
        _nameText.text = _unit.UnitName;

        ChangeCameraPosition(_unit.RenderCameraPoint);

        _unitImage.material = _material;
    }

    public void ResetInfo()
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

    private void UpdateUnit(ref Unit previous, Unit current)
    {
        if(previous != null)
        {
            previous.Damage.MinDamage.OnUpdateValue -= UpdateMinDamageInfo;
            previous.Damage.MaxDamage.OnUpdateValue -= UpdateMaxDamageInfo;
            previous.Damage.CritMultiplyer.OnUpdateValue -= UpdateCritMultiInfo;
            previous.Damage.CritChance.OnUpdateValue -= UpdateCritChanceInfo;
            previous.Health.OnChangeMaxHealth -= UpdateMaxHealth;
            previous.Health.OnChangeCurrentHealth -= UpdateCurrentHealth;
        }

        previous = current;

        previous.Damage.MinDamage.OnUpdateValue += UpdateMinDamageInfo;
        previous.Damage.MaxDamage.OnUpdateValue += UpdateMaxDamageInfo;
        previous.Damage.CritMultiplyer.OnUpdateValue += UpdateCritMultiInfo;
        previous.Damage.CritChance.OnUpdateValue += UpdateCritChanceInfo;
        previous.Health.OnChangeMaxHealth += UpdateMaxHealth;
        previous.Health.OnChangeCurrentHealth += UpdateCurrentHealth;
    }

    private void UpdateMaxHealth(float value)
    {
        _health.Item1 = value;

        _healthText.text = $"{_health.Item2}/{_health.Item1}";
    }
    private void UpdateCurrentHealth(float value)
    {
        _health.Item2 = value;

        _healthText.text = $"{_health.Item2}/{_health.Item1}";
    }

    private void UpdateCritMultiInfo(float value)
    {
        _crit.Item2 = value;

        _critMultiText.text = $"{_crit.Item2 * 100}%";
    }

    private void UpdateCritChanceInfo(float value)
    {
        _crit.Item1 = value;

        _critMultiText.text = $"{_crit.Item1}x";
    }

    private void UpdateMaxDamageInfo(float value)
    {
        _damage.Item2 = value;

        _damageText.text = $"{_damage.Item1}-{_damage.Item2}";
    }

    private void UpdateMinDamageInfo(float value)
    {
        _damage.Item1 = value;
    }

    private void OnDestroy()
    {
        _targetSystem.OnSetTarget -= DrawBaseInfo;
    }
}
