using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class EntityStats : NetworkBehaviour
{
    public event Action<StatType, float> OnStatChange;

    private readonly SyncDictionary<StatType, float> _stats = new();

    public void Initialize(Dictionary<StatType, float> stats)
    {
        foreach (KeyValuePair<StatType, float> stat in stats)
        {
            _stats[stat.Key] = stat.Value;
        }
    }

    public void AddStatMultiplier(StatType type, float value)
        => CmdAddStatMultiplier(type, value);

    public void RemoveStatMultiplier(StatType type, float value)
        => CmdRemoveStatMultiplier(type, value);

    public float GetStatMultiplier(StatType type)
        => _stats[type];

    [Command(requiresAuthority = false)]
    private void CmdAddStatMultiplier(StatType type, float value)
    {
        _stats[type] += value;

        RpcOnStatChange(type, _stats[type]);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveStatMultiplier(StatType type, float value)
    {
        _stats[type] -= value;

        RpcOnStatChange(type, _stats[type]);
    }

    [ClientRpc]
    private void RpcOnStatChange(StatType type, float amount)
        => OnStatChange?.Invoke(type, amount);

    public override string ToString()
    => $"{string.Join(" | ", _stats.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
}
