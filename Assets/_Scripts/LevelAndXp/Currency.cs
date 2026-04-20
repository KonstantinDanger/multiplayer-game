using System;
using UnityEngine;

[Serializable]
public class Currency
{

    [field: SerializeField] public int Amount { get; private set; }
    public event Action<int> OnCurrencyChanged;

    public Currency()
        => Amount = 0;

    public Currency(int amount)
        => Amount = amount;

    public virtual bool Add(int amount)
    {
        if (amount <= 0)
            return false;

        Amount += amount;

        OnCurrencyChanged?.Invoke(amount);

        return true;
    }

    public virtual bool Withdraw(int amount)
    {
        if (amount > Amount)
            return false;

        Amount -= amount;

        OnCurrencyChanged?.Invoke(-amount);

        return true;
    }

    public void Reset()
        => Amount = 0;
}
