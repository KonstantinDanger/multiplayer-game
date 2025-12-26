using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static readonly Dictionary<Comparison, Func<float, float, bool>> ComparisonMap = new()
    {
        [Comparison.Equal] = (a, b) => a == b,
        [Comparison.GreaterThan] = (a, b) => a > b,
        [Comparison.LessThan] = (a, b) => a < b,
    };

    public static bool Compare(float a, float b, Comparison comparisonOption)
    {
        var compare = ComparisonMap[comparisonOption];
        return compare(a, b);
    }

    public static void SpawnTemporarySphere(Vector3 position, float scale, float destroyTime = 1)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (sphere.TryGetComponent(out Collider col))
            GameObject.Destroy(col);

        sphere.GetComponent<Renderer>().material.color = Color.red;
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * scale;

        GameObject.Destroy(sphere, destroyTime);
    }

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
