using System;
using System.Collections.Generic;

public static class Utils
{
    public static Dictionary<StatType, float> DefaultEntityStats()
    {
        Dictionary<StatType, float> stats = new();

        foreach (var item in Enum.GetValues(typeof(StatType)))
            stats[(StatType)item] = 1f;

        return stats;
    }
}
