using System.Collections.Generic;
using UnityEngine;

public class RoomInitializer : MonoBehaviour, IService
{
    private PartyManager _partyManager;
    private EnvironmentGenerator _environmentGenerator;
    private RoomsController _roomsController;
    
    public void Init()
    {
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _environmentGenerator = ServiceLocator.Instance.GetService<EnvironmentGenerator>();
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
    }
    
    public void InitializeRoom()
    {
        _partyManager.InitializePlayerParty(new List<UnitType>() { UnitType.Warrior });
        _partyManager.InitializeEnemyParty(4);
        
        _environmentGenerator.CreateRandom();
    }

    public void UpdateRoom()
    {
        if (_roomsController.IsRecruitsRoom()) UpdateRecruitsRoom();
        else if (_roomsController.IsLastRoom()) UpdateBossRoom();
        else UpdateCommonRoom();
    }

    private void UpdateCommonRoom()
    {
        _partyManager.DestroyDeathPartyMembers();
        _partyManager.HidePlayerParty();
        
        _partyManager.RemoveAllEnemyPartyMembers();
        _partyManager.InitializeEnemyParty(4);
        
        _environmentGenerator.CreateRandom();
    }

    private void UpdateRecruitsRoom()
    {
        _partyManager.DestroyDeathPartyMembers();
        _partyManager.HidePlayerParty();
        
        _partyManager.RemoveAllEnemyPartyMembers();
        _partyManager.InitializeEnemyParty(SaveService.SaveData.PreviousPlayerParty);
        
        _environmentGenerator.CreateRandom();
    }
    
    private void UpdateBossRoom()
    {
        _partyManager.DestroyDeathPartyMembers();
        _partyManager.HidePlayerParty();
        
        _partyManager.RemoveAllEnemyPartyMembers();
        
        var prevPlayer = SaveService.SaveData.PreviousPlayer;
        if (prevPlayer == null) _partyManager.InitializeEnemyParty(new List<UnitType>() { UnitType.Warrior });
        else _partyManager.InitializeEnemyParty(new List<SerializeUnit>() {SaveService.SaveData.PreviousPlayer});
        
        _environmentGenerator.CreateRandom();
    }
}