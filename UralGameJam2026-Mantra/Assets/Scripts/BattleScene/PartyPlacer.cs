using System.Collections.Generic;
using UnityEngine;

public class PartyPlacer : MonoBehaviour
{
    [SerializeField] private bool _placeInAwake = true;

    [SerializeField] private PlaceDirection _direction;

    [SerializeField] private float _gap = .5f;

    //no member's margin yet
    [SerializeField] private List<Transform> _partyMembers = new();
    [SerializeField] private Transform _startPoint;

    private void Awake()
    {
        if(_placeInAwake) PlaceMembers();
    }

    public void PlaceMembers()
    {
        var newPoint = _startPoint.transform.position;

        var direction = _direction == PlaceDirection.Right ? Vector2.right : Vector2.left;

        for (int i = 0; i < _partyMembers.Count; i++)
        {
            var member = _partyMembers[i];

            newPoint.x = _gap * i;
            newPoint *= direction;

            member.transform.position = newPoint;
        }
    }
}

public enum PlaceDirection
{
    Right,
    Left
}
