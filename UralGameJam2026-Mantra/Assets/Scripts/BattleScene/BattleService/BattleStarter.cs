using UnityEngine;

public class BattleStarter : IService
{
    private DialoguePlayer _dialoguePlayer;
    private BattleManager _battleManager;
    
    public BattleStarter(DialoguePlayer dialoguePlayer, BattleManager battleManager)
    {
        _dialoguePlayer = dialoguePlayer;
        _battleManager = battleManager;
    }

    public void StartBattleWithDialogueChance()
    {
        _dialoguePlayer.PlayDialogueWithChance("EnterRoom",1, StartBattle);
    }

    public void StartBattle()
    {
        _battleManager.InitializeBattle();
    }
}