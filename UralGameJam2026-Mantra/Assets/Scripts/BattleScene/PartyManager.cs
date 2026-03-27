using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour, IService
{
    public Party PlayerParty;
    public Party EnemyParty;

    [SerializeField] private PartyPlacer _enemyPartyPlacer;
    [SerializeField] private PartyPlacer _playerPartyPlacer;
    
    public PartyPlacer PlayerPartyPlacer => _playerPartyPlacer;
    
    //[SerializeField] private Unit _memberPrefab;

    [SerializeField] private UnitSelector _unitSelector;
    
    public void InitializePlayerParty(int count)
    {
        var remainingCounts = Mathf.Clamp(count - PlayerParty.Members.Count, 0, PlayerParty.MaxCount);
        
        List<Unit> units = new List<Unit>();
        for (int i = 0; i < remainingCounts; i++)
        {
            var prefab = _unitSelector.RandomSelect();
            var unit = SpawnRandomUnit(prefab, PlayerParty.transform);
            if (i == 0) unit.IsMainHero = true;
            
            units.Add(unit);
        }
        InitializePlayerParty(units);
    }
    
    public void InitializePlayerParty(List<Unit> units)
    {
        PlayerParty.AddMembers(units);
    }

    public void PlacePlayerParty(Action callback = null)
    {
        ShowPlayerParty(false);
        _playerPartyPlacer.PlaceMembersWithTransition(18, .5f, callback);
    }

    public Unit GetMainHero()
    {
        return PlayerParty.Members.FirstOrDefault(m => m.IsMainHero);
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

    public void HidePlayerPartyAuras()
    {
        foreach (var unit in PlayerParty.Members)
        {
            unit.HideAura();
        }
    }

    public void ShowPlayerPartyAuras()
    {
        foreach (var unit in PlayerParty.Members)
        {
            unit.ShowAura();
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

    public void DestroyDeathPartyMembers()
    {
        var deathPartyMembers = PlayerParty.Members.Where(m => !m.IsAlive && !m.IsMainHero).ToList();
        foreach (var deathPartyMember in deathPartyMembers)
        {
            PlayerParty.DestroyMember(deathPartyMember);
        }
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
            var prefab = _unitSelector.RandomSelect();
            var unit = SpawnRandomUnit(prefab, EnemyParty.transform);
            unit.ShouldCreateAura = false;
            units.Add(unit);
        }
        InitializeEnemyParty(units);
    }

    public void InitializeEnemyParty(List<SerializeUnit> unitsData)
    {
        var units = new List<Unit>();
        foreach (var data in unitsData)
        {
            var prefab = _unitSelector.RandomSelect();
            var unit = SpawnUnit(prefab, data);
            unit.ShouldCreateAura = false;
            units.Add(unit);
        }
        InitializeEnemyParty(units);
    }

    private Unit SpawnRandomUnit(Unit prefab, Transform parent)
    {
        var unit = Instantiate(prefab, parent);

        var name = ServiceLocator.Instance.GetService<NameGenerator>().GenerateName();
        unit.SetName(name);

        ServiceLocator.Instance.GetService<StatRandomizer>().InitUnit(unit);
        return unit;
    }
    
    private Unit SpawnUnit(Unit prefab, SerializeUnit unitData)
    {
        var unit = Instantiate(prefab, EnemyParty.transform);
        unit.Deserialize(unitData);
        
        return unit;
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
    
    public bool IsOnEnemyParty(Unit unit)
    {
        return EnemyParty.Members.Contains(unit);
    }

    public bool IsOnPlayerParty(Unit unit)
    {
        return PlayerParty.Members.Contains(unit);
    }
}