using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class UltimateAttackAction : UnitAction
{
    [SerializeField] private float _attackCooldown;
    private float _currentCooldown;

    private void Awake()
    {
        _currentCooldown = 0;
    }

    public override bool CanUse()
    {
        return _currentCooldown == _attackCooldown;
    }

    public void IncreaseCooldown(out float current, out float max)
    {
        if (_currentCooldown < _attackCooldown)
        {
            _currentCooldown = Mathf.Clamp(_currentCooldown + 1, 0, _attackCooldown);
        }
        current = _currentCooldown;
        max = _attackCooldown;
    }

    public override async UniTask Execute(CancellationToken token = default)
    {
        var cached = ActionHelper.DisableTargetSystem();

        if (_person == null)
        {
            _person = GetComponentInParent<Unit>();
        }

        var audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        audioManager.PlaySound("Ulta");
        await UniTask.WaitForSeconds(Random.value, cancellationToken: token);

        if (_skill)
        {
            print($"{_person.UnitName} do ultimate skill on {_target.UnitName}");
            _skill.Use(_target);

            await UniTask.WaitForSeconds(Random.value, cancellationToken: token);

            _animDelay = _skill.UseDelay();
        }

        _currentCooldown = 0;

        await UniTask.WaitForSeconds(_animDelay, cancellationToken: token);

        ActionHelper.EnableTargetSystem(cached);
    }

    public override void Plan(Unit person, Unit target)
    {
        _target = target;
        _person = person;
    }
}
