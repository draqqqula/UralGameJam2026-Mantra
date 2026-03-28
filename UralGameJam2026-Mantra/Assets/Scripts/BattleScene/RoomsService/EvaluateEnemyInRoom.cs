using TMPro;
using UnityEngine;

[System.Serializable]
public class EvaluateEnemyInRoom
{
    [SerializeField] private AnimationCurve _roomProgression;

    [SerializeField] private float _negative, _positive;

    public int GetEnemyCount(int currentRoom, int maxRoom)
    {
        var lastKey = _roomProgression.keys[^1].time = maxRoom;
        int evaluate = Mathf.RoundToInt(_roomProgression.Evaluate(currentRoom));
        int innacuracy = Mathf.RoundToInt(Random.Range(_negative, _positive));

        int result = Mathf.Clamp(evaluate + innacuracy, 1, 4);
        return result;
    }
}