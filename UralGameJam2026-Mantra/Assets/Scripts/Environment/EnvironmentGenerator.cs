using UnityEngine;

public class EnvironmentGenerator : MonoBehaviour, IService
{
    [SerializeField] private Transform _origin;
    [SerializeField] private RoomPool _rooms;
    [SerializeField] private GameObject _lastRoom;
    private GameObject _instance;

    public void CreateRandom()
    {
        var isLastRoom = ServiceLocator.Instance.GetService<RoomsController>().IsLastRoom();

        if (isLastRoom)
        {
            ReplaceRoom(_lastRoom);
            return;
        }

        var id = Random.Range(0, _rooms.Rooms.Count);
        ReplaceRoom(_rooms.Rooms[id]);
    }

    private void ReplaceRoom(GameObject prefab)
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = Instantiate(prefab, _origin);
    }
}
