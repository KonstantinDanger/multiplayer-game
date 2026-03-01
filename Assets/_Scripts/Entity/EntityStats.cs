using System;
using System.Collections.Generic;

public class EntityStats
{
    public Action<StatType, float> OnStatChange;

    private readonly Dictionary<StatType, float> _stats;

    public EntityStats(Dictionary<StatType, float> stats)
        => _stats = stats;

    public void AddStatMultiplier(StatType type, float value)
    {
        _stats[type] += value;

        OnStatChange?.Invoke(type, _stats[type]);
    }

    public void RemoveStatMultiplier(StatType type, float value)
    {
        _stats[type] -= value;

        OnStatChange?.Invoke(type, _stats[type]);
    }

    public float GetStatMultiplier(StatType type, float value)
    => _stats[type];
}
