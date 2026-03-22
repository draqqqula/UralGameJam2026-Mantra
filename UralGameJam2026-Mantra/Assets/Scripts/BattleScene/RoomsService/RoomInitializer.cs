using UnityEngine;

public class RoomInitializer : MonoBehaviour, IService
{
    private PartyManager _partyManager;
    private EnvironmentGenerator _environmentGenerator;
    
    public void Init()
    {
        _partyManager = ServiceLocator.Instance.GetService<PartyManager>();
        _environmentGenerator = ServiceLocator.Instance.GetService<EnvironmentGenerator>();
    }
    
    public void InitializeRoom()
    {
        _partyManager.InitializePlayerParty(4);
        _partyManager.InitializeEnemyParty(4);
        _environmentGenerator.CreateRandom();
    }

    public void UpdateRoom()
    {
        _partyManager.HidePlayerParty();
        _partyManager.RemoveAllEnemyPartyMembers();
        _partyManager.InitializeEnemyParty(4);
        _environmentGenerator.CreateRandom();
    }
}