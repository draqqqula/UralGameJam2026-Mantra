using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room List", menuName = "Room List")]
public class RoomPool : ScriptableObject
{
    [SerializeField] private List<GameObject> _rooms;
    public IReadOnlyList<GameObject> Rooms => _rooms;
}
