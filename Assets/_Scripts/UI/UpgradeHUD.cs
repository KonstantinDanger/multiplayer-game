using UnityEngine;

public class UpgradeHUD : HUD
{
    [SerializeField] private UpgradeNotifier _upgradesNotifier;

    private Upgrader _upgrader;

    public void Initialize(Upgrader upgrader)
    {
        _upgrader = upgrader;
        HandleUpgradeAmountChange(_upgrader.GivenUpgradesCount);
        _upgrader.OnUpgradeAmountChange += HandleUpgradeAmountChange;
    }

    private void OnDestroy()
        => _upgrader.OnUpgradeAmountChange -= HandleUpgradeAmountChange;

    private void HandleUpgradeAmountChange(int upgradesAmount)
        => _upgradesNotifier.gameObject.SetActive(upgradesAmount > 0);
}