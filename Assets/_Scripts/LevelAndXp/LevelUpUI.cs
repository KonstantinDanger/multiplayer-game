using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private Button _levelUpButton;
    [SerializeField] private TextMeshProUGUI _levelPreviewText;
    [SerializeField] private TextMeshProUGUI _levelUpPriceText;

    [SerializeField] private Color _lockedTextColor;

    private Color _defaultTextColor;

    private Level _level;
    private Currency _matchCurrency;

    private void Awake()
        => gameObject.SetActive(false);

    private void OnEnable()
    {
        HandleCurrencyChanged(_matchCurrency.Amount);

        _levelUpButton.onClick.AddListener(HandleLevelUp);
    }

    private void OnDisable()
        => _levelUpButton.onClick.RemoveListener(HandleLevelUp);

    public void Initialize(Level level, Currency matchCurrency)
    {
        _level = level;

        _matchCurrency = matchCurrency;

        _level.OnLevelChanged += HandleLevelChanged;
        _level.OnCurrencyPerLevelChanged += HandleCurrencyPerLevelChanged;
        _matchCurrency.OnCurrencyChanged += HandleCurrencyChanged;

        _defaultTextColor = _levelPreviewText.color;
    }

    private void OnDestroy()
    {
        _level.OnLevelChanged -= HandleLevelChanged;
        _level.OnCurrencyPerLevelChanged -= HandleCurrencyPerLevelChanged;
        _matchCurrency.OnCurrencyChanged -= HandleCurrencyChanged;
    }

    private void HandleCurrencyPerLevelChanged()
        => SetPriceText(IsUpgradeEnabled());

    private void HandleLevelChanged(int level)
        => HandleCurrencyChanged(_matchCurrency.Amount);

    private void HandleLevelUp()
    {
        _level.TryLevelUp();
        HandleCurrencyChanged(0);
    }

    private void HandleCurrencyChanged(int amount)
    {
        bool upgradeEnabled = IsUpgradeEnabled();

        SetButton(upgradeEnabled);
        SetPriceText(upgradeEnabled);
        SetLevelLabel(upgradeEnabled);
    }

    private bool IsUpgradeEnabled()
        => _matchCurrency.Amount >= _level.RequiredCurrencyForUpgrade
        && _level.Lvl < _level.MaxLvl;

    private void SetLevelLabel(bool upgradeEnabled)
    {
        if (IsLevelMaxedOut())
        {
            _levelPreviewText.text = "Max level reached";
            return;
        }

        _levelPreviewText.text = upgradeEnabled ? $"Lvl {_level.Lvl} -> {_level.Lvl + 1}" : $"Lvl {_level.Lvl}";
    }

    private void SetButton(bool upgradeEnabled)
        => _levelUpButton.gameObject.SetActive(upgradeEnabled && !IsLevelMaxedOut());

    private void SetPriceText(bool upgradeEnabled)
    {
        if (IsLevelMaxedOut())
        {
            _levelUpPriceText.gameObject.SetActive(false);
            return;
        }

        _levelUpPriceText.color = upgradeEnabled ? _defaultTextColor : _lockedTextColor;
        _levelUpPriceText.text = $"{_level.RequiredCurrencyForUpgrade}";
    }

    private bool IsLevelMaxedOut()
        => _level.Lvl == _level.MaxLvl;
}
