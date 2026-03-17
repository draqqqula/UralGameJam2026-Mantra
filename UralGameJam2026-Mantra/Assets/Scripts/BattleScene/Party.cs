using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public List<Unit> Members = new();
    [SerializeField] private int _maxCount = 4;

    public void AddMembers(List<Unit> members)
    {
        foreach(var unit  in members)
        {
            AddMember(unit);
        }
    }

    public void RemoveMembers(List<Unit> members)
    {
        foreach( var unit in members)
        {
            RemoveMember(unit);
        }
    }

    public void AddMember(Unit member)
    {
        if(Members.Count == _maxCount) return;
        if(Members.Contains(member)) Members.Remove(member);
        Members.Add(member);
    }

    public void RemoveMember(Unit member)
    {
        if (!Members.Contains(member)) return;
        Members.Remove(member);
    }
}
