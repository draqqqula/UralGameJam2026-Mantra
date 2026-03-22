using System;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour, IService
{
    public Party PlayerParty;
    public Party EnemyParty;

    [SerializeField] private PartyPlacer _enemyPartyPlacer;
    [SerializeField] private PartyPlacer _playerPartyPlacer;
    
    [SerializeField] private Unit _memberPrefab;
    
    public void InitializePlayerParty(int count, Action callback = null)
    {
        var remainingCounts = Mathf.Clamp(count - PlayerParty.Members.Count, 0, PlayerParty.MaxCount);
        
        List<Unit> units = new List<Unit>();
        for (int i = 0; i < remainingCounts; i++)
        {
            var unit = Instantiate(_memberPrefab, PlayerParty.transform);

            var name = ServiceLocator.Instance.GetService<NameGenerator>().GenerateName();
            unit.SetName(name);

            units.Add(unit);
        }
        InitializePlayerParty(units, callback);
    }
    
    public void InitializePlayerParty(List<Unit> units, Action callback = null)
    {
        PlayerParty.AddMembers(units);
        PlacePlayerParty(callback);
    }

    public void PlacePlayerParty(Action callback = null)
    {
        ShowPlayerParty(false);
        _playerPartyPlacer.PlaceMembersWithTransition(18, callback);
    }

    public void HidePlayerParty(bool hideHealthbars = true)
    {
        foreach (var unit in PlayerParty.Members)
        {
            unit.gameObject.SetActive(false);
            if (hideHealthbars) unit.HideHealthbars();
        }
    }

    public void ShowPlayerParty(bool showHealthbars = true)
    {
        foreach (var unit in PlayerParty.Members)
        {
            unit.gameObject.SetActive(true);
            if (showHealthbars) unit.ShowHealthbars();
        }
    }

    public void AddPlayerPartyMember(Unit unit)
    {
        PlayerParty.AddMember(unit);
    }

    public void RemovePlayerPartyMember(Unit unit)
    {
        PlayerParty.RemoveMember(unit);
    }

    public void RemoveAllPlayerPartyMembers()
    {
        PlayerParty.RemoveAllMembers();
    }

    public void InitializeEnemyParty(int count)
    {
        var remainingCounts = Mathf.Clamp(count - EnemyParty.Members.Count, 0, EnemyParty.MaxCount);
        
        List<Unit> units = new List<Unit>();
        for (int i = 0; i < remainingCounts; i++)
        {
            var unit = Instantiate(_memberPrefab, EnemyParty.transform);

            var name = ServiceLocator.Instance.GetService<NameGenerator>().GenerateName();
            unit.SetName(name);
            unit.ShouldShowAura = false;

            units.Add(unit);
        }
        InitializeEnemyParty(units);
    }
    
    public void InitializeEnemyParty(List<Unit> units)
    {
        EnemyParty.AddMembers(units);
        _enemyPartyPlacer.PlaceMembers();
    }

    public void AddEnemyPartyMember(Unit unit)
    {
        EnemyParty.AddMember(unit);
    }
    
    public void RemoveAllEnemyPartyMembers()
    {
        EnemyParty.RemoveAllMembers();
    }

    public void RemoveEnemyPartyMember(Unit unit)
    {
        EnemyParty.RemoveMember(unit);
    }
}
