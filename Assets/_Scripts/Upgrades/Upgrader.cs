using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Upgrader : NetworkBehaviour
{
    public Action<int> ClientOnUpgradeAmountChange;

    [SerializeField] private Level _level;
    [SerializeField] private UpgradeConfig _config;
    [SerializeField] private UpgradeSchema _upgradeSchema;

    private UpgradePicker _picker;

    [SyncVar] private int _givenUpgradesCount;
    [SyncVar] private int _levelCounter;

    public int GivenUpgradesCount => _givenUpgradesCount;

    private void OnEnable()
        => _level.ServerOnLevelChange += HandleLevelChange;

    private void OnDisable()
        => _level.ServerOnLevelChange -= HandleLevelChange;

    public void Initialize()
    {
        IEnumerable<UpgradeNode> upgradeSchema = _upgradeSchema.GetUpgradeSchema();

        _picker = new(upgradeSchema, _config);
    }

    public IEnumerable<ScriptableUpgrade> GiveUpgrades()
    {
        UnityEngine.Debug.Log("Give upgrades ");

        if (_givenUpgradesCount == 0)
            return null;

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
        UnityEngine.Debug.Log("Handle ADD UPGRADE");

        _givenUpgradesCount++;
        RpcOnUpgradeAmountChange(_givenUpgradesCount);
    }

    [ClientRpc]
    private void RpcOnUpgradeAmountChange(int upgradeAmount)
        => ClientOnUpgradeAmountChange?.Invoke(upgradeAmount);
}
