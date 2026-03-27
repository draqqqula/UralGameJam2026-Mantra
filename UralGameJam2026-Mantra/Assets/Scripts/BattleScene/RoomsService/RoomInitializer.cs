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
        _partyManager.InitializePlayerParty(4);
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
        _partyManager.InitializeEnemyParty(1);
        
        _environmentGenerator.CreateRandom();
    }
}