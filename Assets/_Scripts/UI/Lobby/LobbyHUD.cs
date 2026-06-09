using TMPro;
using UnityEngine;

public class LobbyHUD : HUD
{
    [SerializeField] private TextMeshProUGUI _currencyText;

    private Wallet _wallet;

    public void Initialize(Wallet wallet)
    {
        _wallet = wallet;

        _wallet.ClientOnCurrencyChange += HandleCurrencyChange;

        UpdateCurrencyText();
    }

    private void OnEnable()
    {
        if (_wallet == null)
            return;

        UpdateCurrencyText();
    }

    private void OnDisable()
    {
        if (_wallet == null)
            return;

        _wallet.ClientOnCurrencyChange -= HandleCurrencyChange;
    }

    private void HandleCurrencyChange(CurrencyType type, int arg2, int arg3)
    {
        if (type != CurrencyType.Meta)
            return;

        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
        => _currencyText.text = _wallet.GetAmountOf(CurrencyType.Meta).ToString();
}

