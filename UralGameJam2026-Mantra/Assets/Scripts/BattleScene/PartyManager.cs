using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    public Party PlayerParty;
    public Party EnemyParty;

    public void InitializePlayerParty(List<Unit> units)
    {
        PlayerParty.AddMembers(units);
    }

    public void AddPlayerPartyMember(Unit unit)
    {
        PlayerParty.AddMember(unit);
    }

    public void RemovePlayerPartyMember(Unit unit)
    {
        PlayerParty.RemoveMember(unit);
    }

    public void InitializeEnemyParty(List<Unit> units)
    {
        EnemyParty.AddMembers(units);
    }

    public void AddEnemyPartyMember(Unit unit)
    {
        EnemyParty.AddMember(unit);
    }

    public void RemoveEnemyPartyMember(Unit unit)
    {
        EnemyParty.RemoveMember(unit);
    }
}
