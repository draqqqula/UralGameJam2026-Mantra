using System.Collections.Generic;
using UnityEngine;

public class PartyPlacer : MonoBehaviour
{
    [SerializeField] private bool _placeInAwake = true;

    [SerializeField] private PlaceDirection _direction;

    [Range(1f, 10f)]
    [SerializeField] private float _gap = 1.25f;

    [SerializeField] private List<PartyMemberGap> _partyMembers = new();
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

            if (i == 0)
            {
                newPoint.x = _gap * i;
                newPoint *= direction;

                member.transform.position = newPoint;
                continue;
            }

            var previousMember = _partyMembers[i - 1];

            newPoint.x = _gap * i * member.MemberGap;
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
