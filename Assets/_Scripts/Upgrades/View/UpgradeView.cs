using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An instance of upgrade UI should be instantiated within LevelUp UI
/// </summary>
public class UpgradeView : UIView
{
    [SerializeField] private UpgradeCard _cardPrefab;
    [SerializeField] private LayoutGroup _grid;

    private GameObject _owner;
    private Upgrader _upgrader;
    private readonly List<UpgradeCard> _cards = new();

    public void Initialize(GameObject owner, Upgrader upgrader)
    {
        _owner = owner;

        _upgrader = upgrader;

        _upgrader.OnUpgradeAmountChange += HandleUpgradeAmountChange;
    }

    private void OnDestroy()
        => _upgrader.OnUpgradeAmountChange -= HandleUpgradeAmountChange;

    public void AddUpgradeCard(Upgrade upgrade, UpgradeInfo info)
    {
        UpgradeCard card = Instantiate(_cardPrefab, _grid.transform);
        card.Initialize(_owner, upgrade, info, OnObtain);
        _cards.Add(card);
    }

    private void HandleUpgradeAmountChange(int upgradesAmount)
    {
        if (upgradesAmount <= 0)
            return;

        gameObject.SetActive(true);

        List<ScriptableUpgrade> upgrades = _upgrader.GiveUpgrades().ToList();

        upgrades.ForEach(upgrade
            => AddUpgradeCard(
                upgrade.GetNew(),
                upgrade.GetInfo()));
    }

    private void OnObtain()
    {
        foreach (UpgradeCard card in _cards)
            Destroy(card.gameObject);

        _cards.Clear();

        gameObject.SetActive(false);
    }
}
