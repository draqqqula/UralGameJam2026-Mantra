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
    
    [SerializeField] private UnitSelector _unitSelector;
    
    public void InitializePlayerParty(int count)
    {
        var remainingCounts = Mathf.Clamp(count - PlayerParty.Members.Count, 0, PlayerParty.MaxCount);
        
        List<int> datas = Enumerable.Repeat(0, remainingCounts).ToList();
        BaseInitializePlayerParty(datas, _ => SpawnRandomUnit(PlayerParty.transform));
    }

    public void InitializePlayerParty(List<UnitType> unitTypes)
    {
        BaseInitializePlayerParty(unitTypes, type => SpawnRandomUnit(type, PlayerParty.transform));
    }
    
    private void BaseInitializePlayerParty<T>(List<T> datas, Func<T, Unit> spawnAction)
    {
        var units = new List<Unit>();
        foreach (var data in datas)
        {
            var unit = spawnAction.Invoke(data);
            units.Add(unit);
        }
        
        PlayerParty.AddMembers(units);
        if (PlayerParty.Members.Count > 0 && PlayerParty.Members.All(m => !m.IsMainHero))
        {
            PlayerParty.Members[0].IsMainHero = true;
        }
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
        
        List<int> datas = Enumerable.Repeat(0, remainingCounts).ToList();
        BaseInitializeEnemyParty(datas, _ => SpawnRandomUnit(EnemyParty.transform));
    }

    public void InitializeEnemyParty(List<SerializeUnit> unitsData)
    {
        BaseInitializeEnemyParty(unitsData, data => SpawnUnit(data, EnemyParty.transform));
    }

    public void InitializeEnemyParty(List<UnitType> unitTypes)
    {
        BaseInitializeEnemyParty(unitTypes, type => SpawnRandomUnit(type, EnemyParty.transform));
    }
    
    private void BaseInitializeEnemyParty<T>(List<T> datas, Func<T, Unit> spawnAction)
    {
        var units = new List<Unit>();
        foreach (var data in datas)
        {
            var unit = spawnAction.Invoke(data);
            unit.ShouldCreateAura = false;
            units.Add(unit);
        }
        
        EnemyParty.AddMembers(units);
        _enemyPartyPlacer.PlaceMembers();
    }

    private Unit SpawnRandomUnit(Transform parent)
    {
        var prefab = _unitSelector.RandomSelect();
        return SpawnUnitWithRandomStats(prefab, parent);
    }
    
    private Unit SpawnRandomUnit(UnitType unitType, Transform parent)
    {
        var prefab = _unitSelector.SelectUnit(unitType);
        return SpawnUnitWithRandomStats(prefab, parent);
    }

    private Unit SpawnUnitWithRandomStats(Unit prefab, Transform parent)
    {
        var unit = Instantiate(prefab, parent);
        
        var name = ServiceLocator.Instance.GetService<NameGenerator>().GenerateName();
        unit.SetName(name);

        ServiceLocator.Instance.GetService<StatRandomizer>().InitUnit(unit);
        return unit;
    }
    
    private Unit SpawnUnit(SerializeUnit unitData, Transform parent)
    {
        var prefab = _unitSelector.SelectUnit(unitData.Type);
        var unit = Instantiate(prefab, parent);
        unit.Deserialize(unitData);
        
        return unit;
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