using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PartyPlacer : MonoBehaviour
{
    [SerializeField] private PlaceDirection _direction;
    [Range(0f, 2f)]
    [SerializeField] private float _gap = 1f;

    [SerializeField] private Party _partyMembers;
    [SerializeField] private Transform _startPoint;
    
    [SerializeField] private Transform _transitionStartPoint;
    private Coroutine _coroutine;
    
    [SerializeField] private AnimationCurve _movementProgressCurve;
    
    public void PlaceMembersWithTransition(float unitSpeed, float membersDelay, Action callback)
    {
        PlaceMembers();
        var newPoints = _partyMembers.Members.Select(m => m.transform.position).ToArray();
        
        foreach (var member in _partyMembers.Members)
        {
            member.transform.position = _transitionStartPoint.position;
        }
        
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(MoveTransition(newPoints, unitSpeed, membersDelay, callback));
    }

    private IEnumerator MoveTransition(Vector3[] newPoints, float unitSpeed, float membersDelay, Action callback = null)
    {
        var audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        yield return null;
        Sequence sequence = DOTween.Sequence();
        
        for (int i = 0; i < _partyMembers.Members.Count; i++)
        {
            var member = _partyMembers.Members[i];
            var distance = Vector2.Distance(newPoints[i], member.transform.position);

            sequence.Insert(i * membersDelay, member.transform
                    .DOMove(newPoints[i], distance / unitSpeed)
                    .SetEase(_movementProgressCurve)
                    .SetLink(member.gameObject)
                    .OnComplete(() => audioManager.PlaySound("Step")));
        }
        
        yield return sequence.WaitForCompletion();
        callback?.Invoke();
        _coroutine = null;
    }

    public void PlaceMembers()
    {
        Vector2 newPoint = _startPoint.position;
        var direction = _direction == PlaceDirection.Right ? 1f : -1f;
        var prevPoint = newPoint;

        for (int i = 0; i < _partyMembers.Members.Count; i++)
        {
            var partyMember = _partyMembers.Members[i];
            partyMember.UpdateUIPosition();

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

    public void PlaceInParty(Unit unit, int stepsCount = 1)
    {
        var lastUnit = _partyMembers.Members[_partyMembers.Members.Count - 1];
        var direction = _direction == PlaceDirection.Right ? 1f : -1f;
        Vector2 prevPoint = lastUnit.transform.position;
     
        var member = unit.GetComponent<PartyMemberGap>();
        var lastMember = lastUnit.GetComponent<PartyMemberGap>();

        var step = _gap + lastMember.MemberGap + member.MemberGap;
        var newPos = prevPoint + new Vector2(stepsCount * step * direction, 0);
        
        member.transform.SetParent(_partyMembers.transform);
        member.transform.position = newPos;
    }

    public void RotateMember(float direction, ref Unit member)
    {
        if(direction == 1)
        {
            var updateRenderPoint = member.RenderCameraPoint.position;
            updateRenderPoint.z = -updateRenderPoint.z;

            member.RenderCameraPoint.position = updateRenderPoint;

            member.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}

public enum PlaceDirection
{
    Right,
    Left
}