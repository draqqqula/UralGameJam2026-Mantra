using System;
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

    [SerializeField] private UnitSkillInfo _unitSkillInfo;
    [SerializeField] private UnitStatusesInfo _unitStatusesInfo;

    private Unit _unit;

    private (float, float) _damage;
    private (float, float) _health;
    private (float, float) _crit;

    private void Start()
    {
        _unitImage.material = null;
    }

    public void ShowView()
    {
        _viewObject.SetActive(true);
    }

    public void HideView()
    {
        _viewObject.SetActive(false);
    }

    public void DrawBaseInfo(Unit unit)
    {
        UpdateUnit(ref _unit, unit);

        if (_unit == null)
        {
            ResetInfo();

            return;
        }

        _damage.Item1 = Mathf.Round(_unit.Damage.MinDamage.ModValue);
        _damage.Item2 = Mathf.Round(_unit.Damage.MaxDamage.ModValue);

        _health.Item1 = _unit.Health.MaxHealth;
        _health.Item2 = _unit.Health.CurrentHealth;

        _crit.Item1 = _unit.Damage.CritChance.ModValue;
        _crit.Item2 = _unit.Damage.CritMultiplyer.ModValue;

        _healthText.text = $"{_health.Item2}/{_health.Item1}";
        _defenseText.text = Mathf.Round(_unit.Health.CurrentDefense.ModValue).ToString();
        _damageText.text = $"{_damage.Item1}-{_damage.Item2}";
        _critChanceText.text = $"{_crit.Item1 * 100}%";
        _critMultiText.text = $"{_crit.Item2}x";
        _nameText.text = _unit.UnitName;
        
        _unitImage.material = _material;

        _unitSkillInfo.ShowSkill(_unit);
        _unitStatusesInfo.Show(_unit);
    }

    private void Update()
    {
        if (_unit == null || !_viewObject.activeInHierarchy) return;
        ChangeCameraPosition(_unit.RenderCameraPoint);
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

        _unitSkillInfo.Hide();
        _unitStatusesInfo.Hide();
    }

    public bool IsEmpty()
    {
        return _unit;
    }

    private void ChangeCameraPosition(Transform point)
    {
        _renderTextureCamera.transform.localPosition = new Vector3(point.position.x, point.position.y, _renderTextureCamera.transform.localPosition.z);
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

        _healthText.text = $"{Mathf.Round(_health.Item2)}/{Mathf.Round(_health.Item1)}";
    }
    private void UpdateCurrentHealth(float value)
    {
        _health.Item2 = value;

        _healthText.text = $"{Mathf.Round(_health.Item2)}/{Mathf.Round(_health.Item1)}";
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

        _damageText.text = $"{Mathf.Round(_damage.Item1)}-{Mathf.Round(_damage.Item2)}";
    }

    private void UpdateMinDamageInfo(float value)
    {
        _damage.Item1 = value;

        _damageText.text = $"{Mathf.Round(_damage.Item1)}-{Mathf.Round(_damage.Item2)}";
    }
}
