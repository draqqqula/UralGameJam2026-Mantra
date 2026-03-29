using UnityEngine;

public class InfoViewController : MonoBehaviour, IService
{
    [SerializeField] private UnitBaseInfoView _initiator;
    [SerializeField] private UnitBaseInfoView _target;

    [SerializeField] private GameObject _viewObject;

    private TargetSystem _targetSystem;
    private BattleManager _battleManager;
    private MatchManager  _matchManager;
    private RecruitingSystem _recruitingSystem;

    private void Start()
    {
        _battleManager = ServiceLocator.Instance.GetService<BattleManager>();
        _targetSystem = TargetSystem.Instance;
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();

        _targetSystem.OnSetTarget += ShowInfo;
        _target.HideView();
    }

    public void Show()
    {
        _viewObject.SetActive(true);
    }

    public void ResetInfo()
    {
        _initiator.ResetInfo();
        _target.ResetInfo();
        _target.HideView();
    }

    public void Hide()
    {
        _viewObject.SetActive(false);
        _initiator.ResetInfo();
        _target.ResetInfo();
    }

    public void HideInfo()
    {
        _initiator.HideView();
        _target.HideView();
    }

    private void ShowInfo(Targetable target)
    {
        if (_matchManager.CurrentMatchState == MatchManager.State.Battle) ShowInfoInBattle(target);
        else if (_matchManager.CurrentMatchState == MatchManager.State.Recrouting) ShowInfoInRecruiting(target);
    }

    private void ShowInfoInBattle(Targetable target)
    {
        if (_battleManager.InitiatorUnit == null) return;

        _initiator.ShowView();
        if (_battleManager.InitiatorUnit.CurrentValue == null)
        {
            _initiator.DrawBaseInfo(target.Unit);
            _target.HideView();
            return;
        }
        
        if (_battleManager.InitiatorUnit.CurrentValue == target.Unit)
        {
            _target.HideView();
            return;
        }

        if (_battleManager.InitiatorUnit.CurrentValue)
        {
            _initiator.DrawBaseInfo(_battleManager.InitiatorUnit.CurrentValue);
            _target.ShowView();
            _target.DrawBaseInfo(target.Unit);
        }
    }

    private void ShowInfoInRecruiting(Targetable target)
    {
        if (!_recruitingSystem.IsChoosingPlayerUnitToSwitch)
        {
            _initiator.ShowView();
            _initiator.DrawBaseInfo(target.Unit);
            _target.HideView();
            return;
        }

        if (_recruitingSystem.IsChoosingPlayerUnitToSwitch)
        {
            _target.ShowView();
            _target.DrawBaseInfo(target.Unit);
        }
    }

    private void OnDestroy()
    {
        _targetSystem.OnSetTarget -= ShowInfo;
    }
}
