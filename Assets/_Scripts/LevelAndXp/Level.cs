using Mirror;
using System;

public class Level : NetworkBehaviour
{
    public event Action ClientOnCurrencyPerLevelChange;
    public Action<int> ServerOnLevelChange;
    public Action<int> ClientOnLevelChange;

    [SyncVar(hook = nameof(RpcOnLevelChange))] private int _level;
    [SyncVar(hook = nameof(RpcOnCurrencyPerLevelChange))] private int _currencyPerLevel;
    [SyncVar] private int _maxLevel;

    public int Lvl => _level;
    public int MaxLvl => _maxLevel;
    public int RequiredCurrencyForUpgrade => _currencyPerLevel;

    private Func<int, float> _currencyPerLevelIncreaseFunction;
    private Wallet _wallet;

    [Server]
    public void Initialize(Func<int, float> xpIncreaseFunc = null, int level = 1, int maxLevel = 10)
    {
        _level = level;
        _maxLevel = maxLevel;

        if (xpIncreaseFunc != null)
            _currencyPerLevelIncreaseFunction = xpIncreaseFunc;
        else
            _currencyPerLevelIncreaseFunction = Utils.DefaultCurrencyIncreaseFormula;

        _currencyPerLevel = GetNextCurrencyPerLevel(level);

        ServerOnLevelChange?.Invoke(level);
    }

    public override void OnStartServer()
        => _wallet = GetComponent<Wallet>();

    public override void OnStartClient()
        => _wallet = GetComponent<Wallet>();

    [Command(requiresAuthority = false)]
    public void TryLevelUp()
        => ServerTryLevelUp();

    [Server]
    private void ServerTryLevelUp()
    {
        if (Lvl >= MaxLvl)
            return;

        if (!_wallet.CanWithdraw(CurrencyType.Match, _currencyPerLevel))
            return;

        _wallet.Withdraw(CurrencyType.Match, _currencyPerLevel);

        LevelUp();
    }

    private void LevelUp()
    {
        _level++;
        ServerOnLevelChange?.Invoke(_level);
        //RpcOnLevelChange(Lvl);

        _currencyPerLevel = GetNextCurrencyPerLevel(Lvl);
        //RpcOnCurrencyPerLevelChange();

    }

    private int GetNextCurrencyPerLevel(int level)
    {
        if (_currencyPerLevelIncreaseFunction == null)
            _currencyPerLevelIncreaseFunction = Utils.DefaultCurrencyIncreaseFormula;

        return (int)_currencyPerLevelIncreaseFunction(level);
    }

    private void RpcOnCurrencyPerLevelChange(int old, int newCurr) => ClientOnCurrencyPerLevelChange?.Invoke();
    private void RpcOnLevelChange(int old, int level) => ClientOnLevelChange?.Invoke(level);
}