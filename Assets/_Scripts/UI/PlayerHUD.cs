using TMPro;
using UnityEngine;

public class PlayerHUD : UI
{
    [SerializeField] private AbilitiesHUD _abilitiesHUD;
    [SerializeField] private GaugeBar _healthGauge;
    [SerializeField] private GaugeBar _xpGauge;
    [SerializeField] private TextMeshProUGUI _levelText;

    private Level _level;

    public void Initialize(AbilityUser abilities, IDamageable damageable, Level level)
    {
        _level = level;

        _abilitiesHUD.Initialize(abilities);
        _healthGauge.Initialize(gauge: damageable);
        _xpGauge.Initialize(gauge: _level);

        _level.OnValueChanged += HandleLevelChanged;

        HandleLevelChanged();
    }

    private void OnDestroy()
        => _level.OnValueChanged -= HandleLevelChanged;

    private void HandleLevelChanged()
        => _levelText.text = $"Lvl: {_level.Lvl} / {_level.MaxLvl}";
}
