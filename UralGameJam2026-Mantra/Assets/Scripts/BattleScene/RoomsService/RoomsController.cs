using System;
using UnityEngine;

public class RoomsController : MonoBehaviour, IService
{
    [field: SerializeField] public int CurrentRoom {get; private set;}
    [field: SerializeField] public int RoomsCount {get; private set;}
    
    private bool _isHaveRecruitsRoom = false;
    
    public event Action<int> OnRoomUpdated;

    public void Init()
    {
        if (SaveService.SaveData.PreviousPlayerParty.Count > 0)
        {
            _isHaveRecruitsRoom = true;
            RoomsCount += 1;
        }
    }
    
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

    public bool IsRecruitsRoom()
    {
        return _isHaveRecruitsRoom && CurrentRoom + 1 == RoomsCount - 1;
    }
}