using System;
using UnityEngine;

public class RoomsController : MonoBehaviour, IService
{
    [field: SerializeField] public int CurrentRoom {get; private set;}
    [field: SerializeField] public int RoomsCount {get; private set;}
    
    public event Action<int> OnRoomUpdated;
    
    public bool TryUpdateCurrentRoom()
    {
        if (!IsLastRoom())
        {
            CurrentRoom++;
            OnRoomUpdated?.Invoke(CurrentRoom);
            return true;
        }
        return false;
    }

    public bool IsLastRoom()
    {
        return CurrentRoom + 1 >= RoomsCount;
    }

    public bool IsFirstRoom()
    {
        return CurrentRoom == 0;
    }
}