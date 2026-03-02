using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private UpgradeCard _cardPrefab;
    [SerializeField] private LayoutGroup _grid;

    public void AddUpgradeCard(Upgrade upgrade)
    {
        UpgradeCard card = Instantiate(_cardPrefab, _grid.transform);
        card.Initialize(upgrade);
    }
}
