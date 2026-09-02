using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : HUD
{
    [SerializeField] private AbilitiesHUD _abilitiesHUD;
    [SerializeField] private DeathHUD _deathHUD;
    [SerializeField] private MatchResultScreenHUD _matchResultHUD;
    [SerializeField] private MatchProgressHUD _matchProgressHUD;
    [SerializeField] private LobbyHUD _lobbyHUD;
    [SerializeField] private GaugeBar _healthGauge;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    [Header("Parent objects")]
    [SerializeField] private GameObject _matchHudParent;

    private Dictionary<Type, HUD> _huds = new();

    private Level _level;
    private Wallet _wallet;

    private void OnEnable()
    {
        if (_level != null)
            HandleLevelChange(_level.Lvl);
    }

    public void Initialize(IAbilityUser abilities, IDamageable damageable, Level level, Respawn respawn, Wallet wallet)
    {
        _level = level;
        _wallet = wallet;

        _abilitiesHUD.Initialize(abilities);
        _healthGauge.Initialize(gauge: damageable);
        _deathHUD.Initialize(respawn);
        _lobbyHUD.Initialize(wallet);

        _level.ClientOnLevelChange += HandleLevelChange;
        _wallet.ClientOnCurrencyChange += HandleCurrencyChange;

        _huds = new()
        {
            [_abilitiesHUD.GetType()] = _abilitiesHUD,
            [_deathHUD.GetType()] = _deathHUD,
            [_matchResultHUD.GetType()] = _matchResultHUD,
            [_matchProgressHUD.GetType()] = _matchProgressHUD,
            [_lobbyHUD.GetType()] = _lobbyHUD,
        };

        if (_huds.ContainsValue(this))
            _huds.Remove(GetType());

        HandleLevelChange(_level.Lvl);
    }

    public T Show<T>() where T : HUD
    {
        Type hudType = typeof(T);

        T hud = (T)_huds[hudType];

        if (!hud.gameObject.activeInHierarchy)
            hud.gameObject.SetActive(true);

        return hud;
    }

    public void Hide<T>() where T : HUD
    {
        Type hudType = typeof(T);

        _huds[hudType].gameObject.SetActive(false);
    }

    public T Get<T>() where T : HUD
        => (T)_huds[typeof(T)];

    public void SetActive(bool active)
        => _matchHudParent.SetActive(active);

    private void OnDestroy()
    {
        _level.ClientOnLevelChange -= HandleLevelChange;
        _wallet.ClientOnCurrencyChange -= HandleCurrencyChange;
    }

    private void HandleLevelChange(int level)
    {
        int maxLvl = _level.MaxLvl == 0 ? 10 : _level.MaxLvl;

        _levelText.text = $"Lvl: {level} / {maxLvl}";
    }

    private void HandleCurrencyChange(CurrencyType type, int delta, int total)
    {
        if (type != CurrencyType.Match)
            return;

        _currencyText.text = total.ToString();
    }
}
