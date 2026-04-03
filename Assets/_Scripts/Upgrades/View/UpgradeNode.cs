using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeNode
{
    [SerializeField] private ScriptableUpgrade _upgradeRef;
    [SerializeField] private List<UpgradeNode> _nextUpgrades = new();
    [field: SerializeField] public bool IsUnlocked { get; private set; }

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
        List<UpgradeNode> copiedNextUpgrades = new();
        _nextUpgrades?.ForEach(n => copiedNextUpgrades.Add(n.Copy()));

        return new(_upgradeRef, copiedNextUpgrades, IsUnlocked);
    }
}
