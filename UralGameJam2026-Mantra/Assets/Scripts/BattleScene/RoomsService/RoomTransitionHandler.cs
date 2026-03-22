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
    private Coroutine _coroutine;

    public void Init()
    {
        _cameraMovementHandler = ServiceLocator.Instance.GetService<CameraMovementHandler>();
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
    }

    public void ActivatePlayerTransition(Action callback = null)
    {
        _partyManager.PlacePlayerParty(callback);
    }
    
    public void ActivateRoomTransition(Action callbackWhenDark = null, Action finishCallback = null)
    {
        if (_coroutine != null) _cameraMovementHandler.StopCoroutine(_coroutine);
        _coroutine = _cameraMovementHandler.StartCoroutine(TransitionRoutine(callbackWhenDark, finishCallback));
    }

    private IEnumerator TransitionRoutine(Action callbackWhenDark = null, Action finishCallback = null)
    {
        yield return _cameraMovementHandler.Move(_slideEndCameraTarget.position, _cameraMovementDuration);
        _cameraMovementHandler.Teleport(_slideStartCameraTarget.position);
        callbackWhenDark?.Invoke();
        yield return _cameraMovementHandler.Move(_cameraRoomTarget.position, _cameraMovementDuration);
        finishCallback?.Invoke();
        _coroutine = null;
    }
}