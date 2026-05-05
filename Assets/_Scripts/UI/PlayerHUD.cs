using TMPro;
using UnityEngine;

public class PlayerHUD : HUD
{
    [SerializeField] private AbilitiesHUD _abilitiesHUD;
    [SerializeField] private GaugeBar _healthGauge;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    private Level _level;
    private Wallet _wallet;

    public void Initialize(AbilityUser abilities, IDamageable damageable, Level level, Upgrader upgrader, Wallet wallet)
    {
        _level = level;
        _wallet = wallet;

        _abilitiesHUD.Initialize(abilities);
        _healthGauge.Initialize(gauge: damageable);

        _level.OnCurrencyPerLevelChanged += HandleLevelChanged;
        _wallet.OnCurrencyChange += HandleCurrencyChanged;

        HandleLevelChanged();
    }

    private void OnDestroy()
    {
        _level.OnCurrencyPerLevelChanged -= HandleLevelChanged;
        _wallet.OnCurrencyChange -= HandleCurrencyChanged;
    }

    private void HandleLevelChanged()
    => _levelText.text = $"Lvl: {_level.Lvl} / {_level.MaxLvl}";

    private void HandleCurrencyChanged(CurrencyType type, int delta, int total)
        => _currencyText.text = total.ToString();
}
