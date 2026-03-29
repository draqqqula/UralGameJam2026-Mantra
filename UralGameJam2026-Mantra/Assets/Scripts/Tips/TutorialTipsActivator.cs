using System;
using R3;
using UnityEngine;

public class TutorialTipsActivator : MonoBehaviour, IService
{
    [SerializeField] private ActionTip _actionTip;
    [SerializeField] private ActionTip _actionTip2;
    
    [SerializeField] private string[] _tips;
    
    private BattleManager _battleManager;
    private MatchManager _matchManager;
    private RoomsController _roomsController;
    private PartyManager _partyManager;
    
    private IDisposable _subscription1;
    private IDisposable _subscription2;
    
    public bool IsActivated { get; private set; }

    public void Init()
    {
        _battleManager = ServiceLocator.Instance.GetService<BattleManager>();
        _matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
    }
    
    public void StartTutorial()
    {
        if (IsActivated) return;
        IsActivated = true;
        
        _actionTip.Show(_tips[0]);
        
        _battleManager.OnCurrentPipelineChanged += OnPipelineChanged;
        _subscription2 = _matchManager.CurrentStateProperty.Subscribe(OnMatchStateChanged);
        _battleManager.OnBattleStarted += OnBattleStarted;
        _partyManager.EnemyParty.OnMembersEmpty += OnEnemyFinished;
    }

    private void OnPipelineChanged(BattleStrategy battleStrategy)
    {
        _subscription1?.Dispose();
        _subscription1 = _battleManager.InitiatorUnit?.Subscribe(OnInitiatorUnitChoosed);
    }

    private void OnInitiatorUnitChoosed(Unit unit)
    {
        if (_matchManager.CurrentMatchState == MatchManager.State.Battle && unit != null)
        {
            if (_roomsController.CurrentRoom == 0)
            {
                _actionTip2.Hide();
                _actionTip.Show(_tips[1]);
            }
            else
            {
                _actionTip.Hide();
                _actionTip2.Show(_tips[3]);
            }
        }
        else
        {
            _actionTip2.Hide();
            _actionTip.Show(_tips[0]);
        }
    }
    
    private void OnMatchStateChanged(MatchManager.State matchState)
    {
        _actionTip2.Hide();
        if (matchState == MatchManager.State.Recrouting)
        {
            _actionTip.Show(_tips[2]);
        }
    }

    private void OnBattleStarted()
    {
        if (_roomsController.CurrentRoom >= 2) StopTutorial();
        else _actionTip.Show(_tips[0]);
    }

    private void OnEnemyFinished()
    {
        _actionTip2.Hide();
        _actionTip.Hide();
    }

    public void StopTutorial()
    {
        if (!IsActivated) return;
        IsActivated = false;
        
        _subscription1?.Dispose();
        _subscription2?.Dispose();
        _battleManager.OnBattleStarted -= OnBattleStarted;
        _battleManager.OnCurrentPipelineChanged -= OnPipelineChanged;
        _partyManager.EnemyParty.OnMembersEmpty -= OnEnemyFinished;
    }

    private void OnDestroy()
    {
        if (!IsActivated) return;
        
        _subscription1?.Dispose();
        _subscription2?.Dispose();
        _battleManager.OnBattleStarted -= OnBattleStarted;
        _battleManager.OnCurrentPipelineChanged -= OnPipelineChanged;
        _partyManager.EnemyParty.OnMembersEmpty -= OnEnemyFinished;
    }
}