using DG.Tweening;
using UnityEngine;

public class CameraMovementHandler : MonoBehaviour, IService
{
    public YieldInstruction Move(Vector2 pos, float duration)
    {
        var updatedPos = new Vector3(pos.x, pos.y, transform.position.z);
        
        return transform
            .DOMove(updatedPos, duration)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnKill(() => Teleport(updatedPos))
            .WaitForCompletion();
    }

    public void Teleport(Vector2 pos)
    {
        var updatedPos = new Vector3(pos.x, pos.y, transform.position.z);
        transform.position = updatedPos;
    }
}
