using UnityEngine;

public class BattleStarter : IService
{
    private const int DialogueChance = 80;
    
    private DialoguePlayer _dialoguePlayer;
    private BattleManager _battleManager;
    
    public BattleStarter(DialoguePlayer dialoguePlayer, BattleManager battleManager)
    {
        _dialoguePlayer = dialoguePlayer;
        _battleManager = battleManager;
    }

    public void StartBattleWithDialogueChance()
    {
        _dialoguePlayer.PlayDialogueWithChance("EnterRoom", DialogueChance, 1, StartBattle);
    }

    public void StartBattle()
    {
        _battleManager.InitializeBattle();
    }
}