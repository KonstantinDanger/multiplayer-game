using System;
using UnityEngine;

[Serializable]
[Obsolete("Replaced with dictionary <CurrencyType, int (value)> within Wallet class")]
public class Currency
{
    [field: SerializeField] public int Amount { get; private set; }

    public Currency()
        => Amount = 0;

    public Currency(int amount)
        => Amount = amount;

    public virtual bool Add(int amount)
    {
        if (amount <= 0)
            return false;

        Amount += amount;

        return true;
    }

    public virtual bool Withdraw(int amount)
    {
        if (amount > Amount)
            return false;

        Amount -= amount;

        return true;
    }

    public void Reset()
        => Amount = 0;

    public override string ToString() => $"Amount: {Amount}";
}
