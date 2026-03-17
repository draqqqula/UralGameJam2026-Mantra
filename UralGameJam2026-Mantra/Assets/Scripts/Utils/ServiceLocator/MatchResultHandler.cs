using UnityEngine;

public class MatchResultHandler : MonoBehaviour, IService
{
    [field: SerializeField] public Result MatchResult { get; set; }
    
    public enum Result {NotFinished, Victory, Defeat}
}