using UnityEngine;

public class BattleStarter : IService
{
    private DialoguePlayer _dialoguePlayer;
    private BattleManager _battleManager;
    private RoomsController _roomsController;
    
    public BattleStarter(DialoguePlayer dialoguePlayer, BattleManager battleManager, RoomsController roomsController)
    {
        _dialoguePlayer = dialoguePlayer;
        _battleManager = battleManager;
        _roomsController = roomsController;
    }

    public void StartBattleWithDialogueChance()
    {
        if (_roomsController.IsRecruitsRoom()) _dialoguePlayer.PlayDialogueWithChance("PrevRecruits", 1, StartBattle);
        else if (_roomsController.IsLastRoom()) _dialoguePlayer.PlayDialogueWithChance("King", 1, StartBattle);
        else _dialoguePlayer.PlayDialogueWithChance("EnterRoom",1, StartBattle);
    }

    public void StartBattle()
    {
        _battleManager.InitializeBattle();
    }
}