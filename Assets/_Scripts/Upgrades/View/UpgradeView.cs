using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private UpgradeCard _cardPrefab;
    [SerializeField] private LayoutGroup _grid;

    private Upgrader _upgrader;

    public void Initialized(Upgrader upgrader)
        => _upgrader = upgrader;

    private void OnEnable()
    {
        if (_upgrader == null)
            return;

        _upgrader.OnUpgradeAmountChange += HandleUpgradeAmountChange;
    }

    public void AddUpgradeCard(Upgrade upgrade)
    {
        UpgradeCard card = Instantiate(_cardPrefab, _grid.transform);
        card.Initialize(upgrade);
    }

    private void HandleUpgradeAmountChange(int upgradesAmount)
    {

    }
}
