using System;
using UnityEngine;
using UnityEngine.UI;

public class NextRoomWindow : Window
{
    public void NextRoom()
    {
        var matchManager = ServiceLocator.Instance.GetService<MatchManager>();
        matchManager.DeclareNextBattle();
        
        DeactivateWindow();
    }
}