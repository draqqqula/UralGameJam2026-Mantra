using R3;
using UnityEngine;

public class RecruitingTipActivator : MonoBehaviour, IService
{
    private RecruitingSystem _recruitingSystem;
    private MatchManager _matchManager;
    [SerializeField] private ActionTip _actionTip;
    
    [SerializeField] private string playerUnitTip;
    [SerializeField] private string enemyUnitTip;
    
    private void Awake()
    {
        _recruitingSystem = ServiceLocator.Instance.GetService<RecruitingSystem>();
        _recruitingSystem.OnUnitChoosed += Show;
        
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _matchManager.CurrentStateProperty.Subscribe(OnMatchStateChanged).AddTo(this);
    }

    private void OnMatchStateChanged(MatchManager.State matchState)
    {
        if (matchState == MatchManager.State.Recrouting) Show();
        else Hide();
    }
    
    private void Show()
    {
        if (_recruitingSystem.IsChoosingPlayerUnitToSwitch) ShowChoosingPlayerUnitTip();
        else ShowChoosingEnemyUnitTip();
    }

    private void ShowChoosingPlayerUnitTip()
    {
        _actionTip.Show(playerUnitTip);
    }

    private void ShowChoosingEnemyUnitTip()
    {
        _actionTip.Show(enemyUnitTip);
    }

    private void Hide()
    {
        if (!_actionTip.IsShowed) return;
        _actionTip.Hide();
    }

    private void OnDestroy()
    {
        _recruitingSystem.OnUnitChoosed -= Show;
    }
}