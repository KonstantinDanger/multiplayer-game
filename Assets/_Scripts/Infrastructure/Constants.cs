using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    private static Dictionary<StatType, string> StatNames = new()
    {
        [StatType.Health] = "Health",
        [StatType.Damage] = "Damage",
        [StatType.Defense] = "Defense",
        [StatType.MoveSpeed] = "Movement speed",
        [StatType.JumpHeight] = "Jump height",
        [StatType.AllProjectileSpeed] = "Speed of all projectiles",
        [StatType.AllProjectileRadius] = "Radius of all projectiles",
        [StatType.AllSplashRadius] = "Radius of all splash attacks",
    };

    public static float SecondsInMinute = 60f;
    public static int PlayerLayer = LayerMask.NameToLayer("Player");
    public static int EnemyLayer = LayerMask.NameToLayer("Enemy");

    public static string NameOfStat(StatType type)
    {
        if (!StatNames.ContainsKey(type))
            return $"Unknown for type of stat \"{type}\"";

        return StatNames[type];
    }
}
