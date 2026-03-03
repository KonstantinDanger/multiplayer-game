using UnityEngine;

public class UpgradeHUD : MonoBehaviour
{
    [SerializeField] private GameObject _availableUpgradesNotifier;

    private Upgrader _upgrader;

    public void Initialize(Upgrader upgrader)
    {
        _upgrader = upgrader;

        _upgrader.OnUpgradeAmountChange += HandleUpgradeAmountChange;
    }

    private void OnDestroy()
        => _upgrader.OnUpgradeAmountChange -= HandleUpgradeAmountChange;

    private void HandleUpgradeAmountChange(int upgradesAmount)
        => _availableUpgradesNotifier.SetActive(upgradesAmount > 0);
}