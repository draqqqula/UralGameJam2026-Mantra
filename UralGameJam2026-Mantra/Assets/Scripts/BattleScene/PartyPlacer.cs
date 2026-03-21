using UnityEngine;

public class PartyPlacer : MonoBehaviour
{
    [SerializeField] private bool _placeInAwake = true;
    [SerializeField] private bool _placeInUpdate = true;

    [SerializeField] private PlaceDirection _direction;
    [Range(0f, 2f)]
    [SerializeField] private float _gap = 1f;

    [SerializeField] private Party _partyMembers;
    [SerializeField] private Transform _startPoint;
    
    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if(_placeInUpdate) PlaceMembers();
#endif
    }

    public void PlaceMembers()
    {
        Vector2 newPoint = _startPoint.position;
        var direction = _direction == PlaceDirection.Right ? 1f : -1f;
        var prevPoint = newPoint;

        for (int i = 0; i < _partyMembers.Members.Count; i++)
        {
            var partyMember = _partyMembers.Members[i];
            partyMember.UpdateHealthbarPosition();

            RotateMember(direction, ref partyMember);
            var member = partyMember.GetComponent<PartyMemberGap>();

            if (i == 0)
            {
                member.transform.position = newPoint;
                prevPoint = newPoint;
                continue;
            }

            var previousMember = partyMember.GetComponent<PartyMemberGap>();

            var step = _gap + previousMember.MemberGap + member.MemberGap;
            var newPos = prevPoint + new Vector2(step * direction, 0);

            prevPoint = newPos;
            member.transform.position = newPos;
        }
    }

    private void RotateMember(float direction, ref Unit member)
    {
        if(direction == -1)
        {
            member.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}

public enum PlaceDirection
{
    Right,
    Left
}