using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeNode
{
    [SerializeField] private ScriptableUpgrade _upgradeRef;
    [SerializeReference, SubclassSelector] private List<UpgradeNode> _nextUpgrades = new();
    [field: SerializeField] public bool IsUnlocked { get; private set; }

    public UpgradeNode()
    {
        _upgradeRef = null;
        _nextUpgrades = new();
        IsUnlocked = false;
    }

    private UpgradeNode(ScriptableUpgrade upgradeRef, List<UpgradeNode> nextUpgrades, bool isUnlocked)
    {
        _upgradeRef = upgradeRef;
        _nextUpgrades = nextUpgrades;
        IsUnlocked = isUnlocked;
    }

    public ScriptableUpgrade Upgrade => _upgradeRef;
    public IReadOnlyList<UpgradeNode> NextUpgrades => _nextUpgrades;

    public void Unlock()
        => IsUnlocked = true;

    public UpgradeNode Copy()
    {
        if (_nextUpgrades.Count == 0)
            return null;

        List<UpgradeNode> copiedNextUpgrades = new();
        _nextUpgrades?.ForEach(node =>
        {
            if (node == null)
                return;

            copiedNextUpgrades.Add(node.Copy());
        });

        return new(_upgradeRef, copiedNextUpgrades, IsUnlocked);
    }
}
