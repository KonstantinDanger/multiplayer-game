using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradePicker
{
    private readonly IEnumerable<UpgradeNode> _schema;
    private readonly UpgradeConfig _config;

    public UpgradePicker(IEnumerable<UpgradeNode> schema, UpgradeConfig config)
    {
        _schema = schema;
        _config = config;
    }

    public IEnumerable<Upgrade> GetRandomizedAvailableUpgrades()
    {
        Upgrade[] availables = GetAvailableUpgrades().ToArray();

        if (availables.Count() <= _config.MaxUpgradesAtTime)
            return availables;

        Upgrade[] shuffled = new Upgrade[_config.MaxUpgradesAtTime];

        for (int i = 0; i < availables.Length - 1; i++)
        {
            int rnd = Random.Range(i, availables.Length);

            Upgrade tmp = availables[i];
            availables[i] = availables[rnd];
            availables[rnd] = tmp;
        }

        for (int i = 0; i < _config.MaxUpgradesAtTime - 1; i++)
            shuffled[i] = availables[i];

        return shuffled;
    }

    private IEnumerable<Upgrade> GetAvailableUpgrades()
    {
        var result = new List<Upgrade>();
        var visited = new HashSet<UpgradeNode>();

        foreach (var node in _schema)
        {
            Traverse(node, result, visited);
        }

        return result;
    }

    private static void Traverse(UpgradeNode node, List<Upgrade> result, HashSet<UpgradeNode> visited)
    {
        if (node == null)
            return;

        if (visited.Contains(node))
            return;

        visited.Add(node);

        if (node.IsUnlocked)
        {
            result.Add(node.Upgrade.GetNew());
            return;
        }

        foreach (var child in node.NextUpgrades)
        {
            Traverse(child, result, visited);
        }
    }
}