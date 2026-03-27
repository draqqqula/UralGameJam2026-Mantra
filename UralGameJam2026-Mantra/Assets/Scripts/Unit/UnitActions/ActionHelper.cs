using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public static class ActionHelper
{
    public static MatchManager.State DisableTargetSystem()
    {
        var match = ServiceLocator.Instance.GetService<MatchManager>();
        var cached = match.CurrentMatchState;
        TargetSystem.Instance.TrySetTarget(null);
        match.CurrentMatchState = MatchManager.State.Waiting;
        return cached;
    }

    public static void EnableTargetSystem(MatchManager.State cached)
    {
        var match = ServiceLocator.Instance.GetService<MatchManager>();
        match.CurrentMatchState = cached;
    }
}