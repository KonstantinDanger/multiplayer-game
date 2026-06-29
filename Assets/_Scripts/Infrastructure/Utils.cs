using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void StartSplashDamageView(float splashRadius, Vector3 position)
    {
        var viewPrefab = Resources.Load<SplashDamageView>("Prefabs/SplashDamageView");
        var viewInstance = GameObject.Instantiate(viewPrefab, position, Quaternion.identity);
        viewInstance.StartView(splashRadius);
    }

    public static string ConvertSecondsToTimerFormat(float secondsElapsed)
    {
        int seconds = Mathf.FloorToInt(secondsElapsed % 60);
        int minutes = Mathf.FloorToInt(seconds / 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public static string GetLayerNamesFromMask(LayerMask mask)
    {
        string str = "";

        for (int i = 0; i < 32; i++)
            if ((mask.value & (1 << i)) != 0)
                str += $"{LayerMask.LayerToName(i)} | ";

        return str;
    }

    public static T GetInstancedCopyOf<T>(T target) where T : class
    {
        if (target == null)
            return null;

        string json = JsonUtility.ToJson(target);
        T clone = JsonUtility.FromJson(json, target.GetType()) as T;

        return clone;
    }

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

    public static Func<int, float> DefaultCurrencyIncreaseFormula = (lvl) =>
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
