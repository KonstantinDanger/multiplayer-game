public class SummonCurrency : Currency
{
    private readonly Currency _ownerCurrency;

    public SummonCurrency(Currency ownerMatchCurrency)
        => _ownerCurrency = ownerMatchCurrency;

    public override bool Add(int amount)
        => _ownerCurrency.Add(amount);

    public override bool Withdraw(int amount)
        => _ownerCurrency.Withdraw(amount);
}
