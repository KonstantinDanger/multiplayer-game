using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private UpgradeCard _cardPrefab;
    [SerializeField] private LayoutGroup _grid;

    private Upgrader _upgrader;

    public void Initialize(Upgrader upgrader)
        => _upgrader = upgrader;

    private void OnEnable()
    {
        if (_upgrader == null)
            return;

        _upgrader.OnUpgradeAmountChange += HandleUpgradeAmountChange;
    }

    private void OnDisable()
        => _upgrader.OnUpgradeAmountChange -= HandleUpgradeAmountChange;

    public void AddUpgradeCard(Upgrade upgrade)
    {
        UpgradeCard card = Instantiate(_cardPrefab, _grid.transform);
        card.Initialize(upgrade);
    }

    private void HandleUpgradeAmountChange(int upgradesAmount)
    {

    }
}
