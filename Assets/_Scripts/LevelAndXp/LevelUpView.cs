using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpView : UIView
{
    [SerializeField] private Button _levelUpButton;
    [SerializeField] private TextMeshProUGUI _levelPreviewText;
    [SerializeField] private TextMeshProUGUI _levelUpPriceText;

    [SerializeField] private Color _lockedTextColor;

    private Color _defaultTextColor;

    private Level _level;
    private Wallet _wallet;

    private const CurrencyType CurrencyTypeToWithdraw = CurrencyType.Match;

    private void Awake()
        => gameObject.SetActive(false);

    private void OnEnable()
    {
        HandleCurrencyChange();

        SetButton(IsUpgradeEnabled());

        _levelUpButton.onClick.AddListener(HandleLevelUp);
    }

    private void OnDisable()
        => _levelUpButton.onClick.RemoveListener(HandleLevelUp);

    public void Initialize(Level level, Wallet wallet)
    {
        _level = level;
        _wallet = wallet;

        _level.ClientOnLevelChange += HandleLevelChange;
        _level.ClientOnCurrencyPerLevelChange += HandleCurrencyPerLevelChange;
        _wallet.ClientOnCurrencyChange += HandleCurrencyChange;

        _defaultTextColor = _levelPreviewText.color;
    }

    private void OnDestroy()
    {
        _level.ClientOnLevelChange -= HandleLevelChange;
        _level.ClientOnCurrencyPerLevelChange -= HandleCurrencyPerLevelChange;
        _wallet.ClientOnCurrencyChange -= HandleCurrencyChange;
    }

    private void HandleCurrencyPerLevelChange()
        => SetPriceText(IsUpgradeEnabled());

    private void HandleLevelChange(int level)
        => HandleCurrencyChange();

    private void HandleLevelUp()
    {
        _level.TryLevelUp();
        HandleCurrencyChange();
    }

    private void HandleCurrencyChange(CurrencyType type = 0, int delta = 0, int total = 0)
    {
        bool upgradeEnabled = IsUpgradeEnabled();

        SetButton(upgradeEnabled);
        SetPriceText(upgradeEnabled);
        SetLevelLabel(upgradeEnabled);
    }

    private bool IsUpgradeEnabled()
        => _wallet.GetAmount(CurrencyTypeToWithdraw) >= _level.RequiredCurrencyForUpgrade
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
