using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Upgrader : NetworkBehaviour
{
    public Action<int> OnUpgradeAmountChange;

    [SerializeField] private Level _level;
    [SerializeField] private UpgradeConfig _config;
    [SerializeField] private UpgradeSchema _upgradeSchema;

    private UpgradePicker _picker;

    private int _givenUpgradesCount;

    private int _levelCounter;

    public int GivenUpgradesCount => _givenUpgradesCount;

    private void OnEnable()
        => _level.OnLevelChanged += HandleLevelChange;

    private void OnDisable()
        => _level.OnLevelChanged -= HandleLevelChange;

    public void Initialize()
    {
        var upgradeSchema = _upgradeSchema.GetUpgradeSchema();

        _picker = new(upgradeSchema, _config);
    }

    public IEnumerable<Upgrade> GiveUpgrade()
    {
        if (_givenUpgradesCount == 0)
            return null;

        _givenUpgradesCount--;
        OnUpgradeAmountChange?.Invoke(_givenUpgradesCount);
        return _picker.GetRandomizedAvailableUpgrades();
    }

    private void HandleLevelChange(int level)
    {
        if (level == 1 && _levelCounter == 0)
            return;

        _levelCounter++;

        int clampedLevel = _levelCounter % _config.GiveUpgradeEachLevel;

        if (clampedLevel == 0)
            AddUpgrade();
    }

    private void AddUpgrade()
    {
        _givenUpgradesCount++;
        OnUpgradeAmountChange?.Invoke(_givenUpgradesCount);
    }
}
