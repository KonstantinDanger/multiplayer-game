using TMPro;
using UnityEngine;

public class PlayerHUD : UI
{
    [SerializeField] private AbilitiesHUD _abilitiesHUD;
    [SerializeField] private UpgradeHUD _upgradeHUD;
    [SerializeField] private GaugeBar _healthGauge;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    private Level _level;
    private Currency _matchCurrency;

    public void Initialize(AbilityUser abilities, IDamageable damageable, Level level, Upgrader upgrader, Wallet wallet)
    {
        _level = level;
        _matchCurrency = wallet.MatchCurrency;

        _abilitiesHUD.Initialize(abilities);
        _healthGauge.Initialize(gauge: damageable);
        _upgradeHUD.Initialize(upgrader);

        _level.OnCurrencyPerLevelChanged += HandleLevelChanged;
        _matchCurrency.OnCurrencyChanged += HandleCurrencyChanged;

        HandleLevelChanged();
    }

    private void HandleLevelChanged()
    => _levelText.text = $"Lvl: {_level.Lvl} / {_level.MaxLvl}";

    private void HandleCurrencyChanged(int amount)
        => _currencyText.text = _matchCurrency.Amount.ToString();

    private void OnDestroy()
    {
        _level.OnCurrencyPerLevelChanged -= HandleLevelChanged;
        _matchCurrency.OnCurrencyChanged -= HandleCurrencyChanged;
    }
}
