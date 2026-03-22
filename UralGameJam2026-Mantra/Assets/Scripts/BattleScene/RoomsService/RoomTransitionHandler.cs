using System;
using System.Collections;
using UnityEngine;

public class RoomTransitionHandler : MonoBehaviour, IService
{
    [SerializeField] private Transform _slideEndCameraTarget;
    [SerializeField] private Transform _slideStartCameraTarget;
    [SerializeField] private Transform _cameraRoomTarget;
    
    [SerializeField] private float _cameraMovementDuration;
    
    private CameraMovementHandler _cameraMovementHandler;
    private PartyManager _partyManager;
    private EnvironmentGenerator _environmentGenerator;
    private Coroutine _coroutine;

    private void Awake()
    {
        _cameraMovementHandler = ServiceLocator.Instance.GetService<CameraMovementHandler>();
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _environmentGenerator = ServiceLocator.Instance.GetService<EnvironmentGenerator>();
    }
    
    public void ActivateTransition(Action callback)
    {
        if (_coroutine != null) _cameraMovementHandler.StopCoroutine(_coroutine);
        _coroutine = _cameraMovementHandler.StartCoroutine(TransitionRoutine(callback));
    }

    private IEnumerator TransitionRoutine(Action callback)
    {
        yield return _cameraMovementHandler.Move(_slideEndCameraTarget.position, _cameraMovementDuration);
        _cameraMovementHandler.Teleport(_slideStartCameraTarget.position);
        
        _partyManager.HidePlayerParty();
        _partyManager.RemoveAllEnemyPartyMembers();
        _partyManager.InitializeEnemyParty(4);
        _environmentGenerator.CreateRandom();

        yield return _cameraMovementHandler.Move(_cameraRoomTarget.position, _cameraMovementDuration);
        callback?.Invoke();
        _coroutine = null;
    }
}