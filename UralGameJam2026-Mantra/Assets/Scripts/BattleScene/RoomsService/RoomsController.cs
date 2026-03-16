using System;
using UnityEngine;

public class RoomsController : MonoBehaviour, IService
{
    [field: SerializeField] public int CurrentRoom {get; private set;}
    [field: SerializeField] public int RoomsCount {get; private set;}
    
    public event Action<int> OnRoomUpdated;
    
    public bool TryUpdateCurrentRoom()
    {
        if (CurrentRoom + 1 < RoomsCount)
        {
            CurrentRoom++;
            OnRoomUpdated?.Invoke(CurrentRoom);
            return true;
        }
        return false;
    }
}