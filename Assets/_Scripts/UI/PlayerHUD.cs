using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : HUD
{
    [SerializeField] private AbilitiesHUD _abilitiesHUD;
    [SerializeField] private DeathHUD _deathHUD;
    [SerializeField] private MatchResultScreenHUD _matchResultHud;
    [SerializeField] private GaugeBar _healthGauge;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    private Dictionary<Type, HUD> _huds = new();

    private Level _level;
    private Wallet _wallet;

    private void OnEnable()
    {
        if (_level != null)
            HandleLevelChange(_level.Lvl);
    }

    private void OnDisable() => UnityEngine.Debug.Log("disabled hud ");

    public void Initialize(AbilityUser abilities, IDamageable damageable, Level level, Respawn respawn, Wallet wallet)
    {
        _level = level;
        _wallet = wallet;

        _abilitiesHUD.Initialize(abilities);
        _healthGauge.Initialize(gauge: damageable);
        _deathHUD.Initialize(respawn);

        _level.ClientOnLevelChange += HandleLevelChange;
        _wallet.ClientOnCurrencyChange += HandleCurrencyChange;

        _huds = new()
        {
            [_abilitiesHUD.GetType()] = _abilitiesHUD,
            [_deathHUD.GetType()] = _deathHUD,
            [_matchResultHud.GetType()] = _matchResultHud,
        };

        if (_huds.ContainsValue(this))
            _huds.Remove(GetType());

        HandleLevelChange(_level.Lvl);
    }

    public T Show<T>() where T : HUD
    {
        Type hudType = typeof(T);

        T hud = (T)_huds[hudType];

        hud.gameObject.SetActive(true);

        return hud;
    }

    public void Hide<T>() where T : HUD
    {
        Type hudType = typeof(T);

        _huds[hudType].gameObject.SetActive(false);
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
