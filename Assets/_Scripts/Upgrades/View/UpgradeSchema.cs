using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UpgradeSchema/UpgradeSchema", fileName = "UpgradeSchema_")]
public class UpgradeSchema : ScriptableObject
{
    [SerializeField] private List<UpgradeNode> _upgradeNodes = new();

    public IEnumerable<UpgradeNode> GetUpgradeSchema()
    {
        List<UpgradeNode> nodes = new();
        _upgradeNodes.ForEach(n => nodes.Add(n.Copy()));
        return nodes;
    }
}
