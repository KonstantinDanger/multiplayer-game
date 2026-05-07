public class PlayerSummaryDataBinder
{
    private readonly Player _player;
    private readonly PlayerMatchSummaryData _summaryData;

    private readonly IDamageable _damageable;
    private readonly Level _level;
    private readonly Wallet _wallet;

    public PlayerSummaryDataBinder(Player player, PlayerMatchSummaryData summaryData, Wallet wallet)
    {
        _player = player;
        _summaryData = summaryData;

        _damageable = _player.Damageable;
        _level = _player.GetComponent<Level>();

        _summaryData.UsedClassName = _player.CharacterClass.ClassName;
        _wallet = wallet;
    }

    public void Bind()
    {
        _damageable.ServerOnDamageTaken += HandleDamageTaken;
        _damageable.ServerOnDemise += HandleDemise;
        _level.ServerOnLevelChange += HandleLevelUp;
        _wallet.ServerOnCurrencyChange += HandleCurrencyReceived;
    }

    public void Unbind()
    {
        _damageable.ServerOnDamageTaken -= HandleDamageTaken;
        _damageable.ServerOnDemise -= HandleDemise;
        _level.ServerOnLevelChange -= HandleLevelUp;
        _wallet.ServerOnCurrencyChange -= HandleCurrencyReceived;
    }

    private void HandleDamageTaken(Damage damage)
    {
        if (damage.Amount <= 0)
            return;

        _summaryData.DamageTaken += damage.Amount;
    }

    private void HandleDemise(Damage damage)
    {
        _summaryData.DeathsCount++;
        HandleDamageTaken(damage);
    }

    private void HandleLevelUp(int level)
        => _summaryData.ReachedLevel = level;

    private void HandleCurrencyReceived(CurrencyType type, int delta, int total)
    {
        if (delta <= 0)
            return;

        _summaryData.TotalCurrencyReceived += delta;
    }
}