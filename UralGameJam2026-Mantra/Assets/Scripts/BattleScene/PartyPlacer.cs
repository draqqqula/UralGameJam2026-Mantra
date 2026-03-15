using System.Collections.Generic;
using UnityEngine;

public class PartyPlacer : MonoBehaviour
{
    [SerializeField] private bool _placeInAwake = true;

    [SerializeField] private PlaceDirection _direction;
    [Range(0f, 2f)]
    [SerializeField] private float _gap = 1f;

    [SerializeField] private List<PartyMemberGap> _partyMembers = new();
    [SerializeField] private Transform _startPoint;

    private void Awake()
    {
        if(_placeInAwake) PlaceMembers();
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        PlaceMembers();
#endif
    }

    public void PlaceMembers()
    {
        Vector2 newPoint = _startPoint.position;
        var direction = _direction == PlaceDirection.Right ? 1f : -1f;
        var prevPoint = newPoint;

        for (int i = 0; i < _partyMembers.Count; i++)
        {
            var member = _partyMembers[i];

            if (i == 0)
            {
                member.transform.position = newPoint;
                prevPoint = newPoint;
                continue;
            }

            var previousMember = _partyMembers[i - 1];

            var step = _gap + previousMember.MemberGap + member.MemberGap;
            var newPos = prevPoint + new Vector2(step * direction, 0);

            prevPoint = newPos;
            member.transform.position = newPos;
        }
    }
}

public enum PlaceDirection
{
    Right,
    Left
}