using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public List<Unit> Members = new();
    [SerializeField] private int _maxCount = 4;
    
    public int MaxCount => _maxCount;
    
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

    public void RemoveAllMembers()
    {
        foreach (var unit in Members)
        {
            Destroy(unit.gameObject);
        }
        Members.Clear();
    }

    public void AddMember(Unit member)
    {
        if(Members.Count == _maxCount) return;
        if(Members.Contains(member)) Members.Remove(member);
        Members.Add(member);
    }

    public void InsertMember(int index, Unit member)
    {
        if(Members.Count == _maxCount) return;
        if(Members.Contains(member)) Members.Remove(member);
        Members.Insert(index, member);
    }

    public void RemoveMember(Unit member)
    {
        if (!Members.Contains(member)) return;
        Members.Remove(member);
    }

    public int IndexOfMember(Unit member)
    {
        return Members.IndexOf(member);
    }

    public void DestroyMember(Unit member)
    {
        RemoveMember(member);
        Destroy(member.gameObject);
    }

    public List<Unit> GetRandomMembers(int count)
    {
        if (count > Members.Count)
        {
            Debug.LogWarning($"Can't get {count} members! List have only {Members.Count} members!");
            return null;
        }
        
        var result = new List<Unit>();
        while (result.Count < count)
        {
            var member = GetRandomMember();
            if (!result.Contains(member)) result.Add(member);
        }
        
        return result;
    }

    public Unit GetRandomMember()
    {
        return Members[Random.Range(0, Members.Count)];
    }
}
