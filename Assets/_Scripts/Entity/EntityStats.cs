using System.Collections.Generic;

public struct EntityStats
{
    private readonly Dictionary<StatType, float> _stats;

    public EntityStats(Dictionary<StatType, float> stats)
        => _stats = stats;

    public readonly void AddStatMultiplier(StatType type, float value)
        => _stats[type] += value;

    public readonly void RemoveStatMultiplier(StatType type, float value)
    => _stats[type] -= value;

    public readonly float GetStatMultiplier(StatType type, float value)
    => _stats[type];
}
