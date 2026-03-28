using R3;
using UnityEngine;

public class RecruitingTipActivator : MonoBehaviour, IService
{
    private RecruitingSystem _recruitingSystem;
    private MatchManager _matchManager;
    private PartyManager _partyManager;
    
    [SerializeField] private ActionTip _actionTip;
    
    [SerializeField] private string playerUnitTip;
    [SerializeField] private string enemyUnitTip;
    
    [SerializeField] private Transform _playerTipPoint;
    [SerializeField] private Transform _enemyTipPoint;
    
    private void Awake()
    {
        _recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();
        _recruitingSystem.OnUnitChoosed += OnChoosedUnitChanged;
        _recruitingSystem.OnUnitCancelled += OnChoosedUnitChanged;
        
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _matchManager.CurrentStateProperty.Subscribe(OnMatchStateChanged).AddTo(this);
        
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _partyManager.EnemyParty.OnMembersEmpty += Hide;
    }

    private void OnMatchStateChanged(MatchManager.State matchState)
    {
        if (matchState == MatchManager.State.Recrouting) Show();
        else Hide();
    }

    private void OnChoosedUnitChanged()
    {
        if (_partyManager.EnemyParty.Members.Count <= 0) Hide();
        else Show();
    }
    
    private void Show()
    {
        if (_recruitingSystem.IsChoosingPlayerUnitToSwitch) ShowChoosingPlayerUnitTip();
        else ShowChoosingEnemyUnitTip();
    }

    private void ShowChoosingPlayerUnitTip()
    {
        _actionTip.transform.position = _playerTipPoint.position;
        _actionTip.Show(playerUnitTip);
    }

    private void ShowChoosingEnemyUnitTip()
    {
        _actionTip.transform.position = _enemyTipPoint.position;
        _actionTip.Show(enemyUnitTip);
    }

    private void Hide()
    {
        if (!_actionTip.IsShowed) return;
        _actionTip.Hide();
    }

    private void OnDestroy()
    {
        _recruitingSystem.OnUnitChoosed -= OnChoosedUnitChanged;
        _recruitingSystem.OnUnitCancelled -= OnChoosedUnitChanged;
        _partyManager.EnemyParty.OnMembersEmpty -= Hide;
    }
}