using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class Wallet : NetworkBehaviour
{
    /// <summary>
    /// Event arguments are 
    /// 1) Currency type 
    /// 2) Currency delta (changed amount)
    /// 3) Total left amount
    /// </summary>
    public event Action<CurrencyType, int, int> OnCurrencyChange;

    private readonly SyncDictionary<CurrencyType, int> _currencies = new()
    {
        [CurrencyType.Match] = 0,
        [CurrencyType.Meta] = 0,
    };

    public void Initialize(Dictionary<CurrencyType, int> currencies)
    {
        foreach (KeyValuePair<CurrencyType, int> pair in currencies)
        {
            _currencies[pair.Key] = pair.Value;
        }
    }

    public void Add(CurrencyType type, int amount)
        => CmdAdd(type, amount);

    public void Withdraw(CurrencyType type, int amount)
        => CmdWithdraw(type, amount);

    public void ResetCurrency(CurrencyType type)
        => CmdReset(type);

    public int GetAmount(CurrencyType type)
        => _currencies[type];

    public bool CanWithdraw(CurrencyType type, int remainingAmount)
        => _currencies[type] >= remainingAmount;

    [Command(requiresAuthority = false)]
    private void CmdAdd(CurrencyType type, int amount)
        => ChangeCurrency(type, amount, (a) => _currencies[type] += a);

    [Command(requiresAuthority = false)]
    private void CmdWithdraw(CurrencyType type, int amount)
        => ChangeCurrency(type, amount, (a) => _currencies[type] -= a);

    [Command(requiresAuthority = false)]
    private void CmdReset(CurrencyType type)
    {
        int delta = _currencies[type];

        _currencies[type] = 0;

        RpcOnCurrencyChange(type, delta, _currencies[0]);
    }

    private void ChangeCurrency(CurrencyType type, int amount, Action<int> changeFunc)
    {
        if (amount <= 0)
            return;

        changeFunc(amount);

        RpcOnCurrencyChange(type, amount, _currencies[type]);
    }

    [ClientRpc]
    private void RpcOnCurrencyChange(CurrencyType type, int delta, int totalLeft)
        => OnCurrencyChange?.Invoke(type, delta, totalLeft);

    public override string ToString()
        => $"{string.Join(" | ", _currencies.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}";
}
