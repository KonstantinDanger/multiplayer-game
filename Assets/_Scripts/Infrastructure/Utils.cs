using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Player ServerFindNetPlayerById(uint netId)
    {
        if (!NetworkServer.spawned.TryGetValue(netId, out NetworkIdentity identity))
            throw new Exception("player not found on the server");

        return identity.GetComponent<Player>();
    }

    public static Func<int, float> DefaultXpIncreaseFormula = (lvl) =>
    {
        //TODO: get this formula from static data
        float defaultXpPerLevel = 1000;
        float multiplier = 1.2f;
        return lvl * multiplier * defaultXpPerLevel;
    };

    public static Dictionary<StatType, float> DefaultEntityStats()
    {
        Dictionary<StatType, float> stats = new();

        foreach (var item in Enum.GetValues(typeof(StatType)))
            stats[(StatType)item] = 1f;

        return stats;
    }

    public static GameObject NetIdToGameObject(uint netId)
    {
        NetworkClient.spawned.TryGetValue(netId, out NetworkIdentity netObject);

        return netObject.gameObject;
    }
}
