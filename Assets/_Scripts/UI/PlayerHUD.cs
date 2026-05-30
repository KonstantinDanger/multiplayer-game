using TMPro;
using UnityEngine;

public class PlayerHUD : HUD
{
    [SerializeField] private AbilitiesHUD _abilitiesHUD;
    [SerializeField] private DeathHUD _deathHUD;
    [SerializeField] private GaugeBar _healthGauge;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    private Level _level;
    private Wallet _wallet;

    private void OnEnable()
    {
        if (_level != null)
            HandleLevelChange(_level.Lvl);
    }

    public void Initialize(AbilityUser abilities, IDamageable damageable, Level level, Respawn respawn, Wallet wallet)
    {
        _level = level;
        _wallet = wallet;

        _abilitiesHUD.Initialize(abilities);
        _healthGauge.Initialize(gauge: damageable);
        _deathHUD.Initialize(damageable, respawn);

        _level.ClientOnLevelChange += HandleLevelChange;
        _wallet.ClientOnCurrencyChange += HandleCurrencyChange;

        HandleLevelChange(_level.Lvl);
    }

    private void OnDestroy()
    {
        _level.ClientOnLevelChange -= HandleLevelChange;
        _wallet.ClientOnCurrencyChange -= HandleCurrencyChange;
    }

    private void HandleLevelChange(int level)
        => _levelText.text = $"Lvl: {level} / {_level.MaxLvl}";

    private void HandleCurrencyChange(CurrencyType type, int delta, int total)
        => _currencyText.text = total.ToString();
}
