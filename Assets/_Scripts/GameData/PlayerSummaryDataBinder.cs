public class PlayerSummaryDataBinder
{
    private readonly Player _player;
    private readonly PlayerMatchSummaryData _summaryData;

    private readonly IDamageable _damageable;
    private readonly Level _level;

    public PlayerSummaryDataBinder(Player player, PlayerMatchSummaryData summaryData)
    {
        _player = player;
        _summaryData = summaryData;

        _damageable = _player.Damageable;
        _level = _player.GetComponent<Level>();

        _summaryData.UsedClassName = _player.CharacterClass.ClassName;
    }

    public void Bind()
    {
        _damageable.OnDamageTaken += HandleDamageTaken;
        _damageable.OnDemise += HandleDemise;
        _level.OnLevelChanged += HandleLevelUp;
        _level.OnXpReceived += HandleXpReceived;
    }

    public void Unbind()
    {
        _damageable.OnDamageTaken -= HandleDamageTaken;
        _damageable.OnDemise -= HandleDemise;
        _level.OnLevelChanged -= HandleLevelUp;
        _level.OnXpReceived -= HandleXpReceived;
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

    private void HandleXpReceived(float amount)
    {
        if (amount <= 0)
            return;

        _summaryData.TotalXpReceived += amount;
    }
}