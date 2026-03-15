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
        var newPoint = _startPoint.transform.position;
        var direction = _direction == PlaceDirection.Right ? Vector2.right : Vector2.left;

        var prevPoint = Vector2.zero;

        for (int i = 0; i < _partyMembers.Count; i++)
        {
            var member = _partyMembers[i];

            if (i == 0)
            {
                newPoint.x = 0;

                member.transform.position = newPoint;
                prevPoint = newPoint;
                continue;
            }

            var previousMember = _partyMembers[i - 1];

            newPoint.x = _gap + previousMember.MemberGap + member.MemberGap + prevPoint.x;
            prevPoint = newPoint;

            member.transform.position = newPoint * direction;
        }
    }
}

public enum PlaceDirection
{
    Right,
    Left
}
