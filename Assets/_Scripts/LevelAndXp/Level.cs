using Mirror;
using System;


public class Level : NetworkBehaviour
{
    public event Action OnValueChanged;
    public Action<int> OnLevelChanged;

    [SyncVar(hook = nameof(OnLevelSync))] private int _level;
    [SyncVar(hook = nameof(OnXpPerLevelSync))] private int _currencyPerLevel;
    [SyncVar] private int _maxLevel;

    public int Lvl => _level;
    public int MaxLvl => _maxLevel;
    public int RequiredCurrencyForUpgrade => _currencyPerLevel;

    private Func<int, float> _currencyPerLevelIncreaseFunction;

    [Server]
    public void Initialize(Func<int, float> xpIncreaseFunc = null, int level = 1, int maxLevel = 10)
    {
        _level = level;
        _maxLevel = maxLevel;

        if (xpIncreaseFunc != null)
            _currencyPerLevelIncreaseFunction = xpIncreaseFunc;

        _currencyPerLevel = GetNextCurrencyPerLevel(_level);

        OnLevelChanged?.Invoke(_level);
        OnValueChanged?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void TryLevelUp(Currency currency)
    {
        if (Lvl >= MaxLvl)
            return;

        if (!currency.Withdraw(_currencyPerLevel))
            return;

        LevelUp();
    }

    [Server]
    private void LevelUp()
    {
        _level++;
        _currencyPerLevel = GetNextCurrencyPerLevel(Lvl);
    }

    private int GetNextCurrencyPerLevel(int level)
    {
        if (_currencyPerLevelIncreaseFunction == null)
            _currencyPerLevelIncreaseFunction = Utils.DefaultCurrencyIncreaseFormula;

        return (int)_currencyPerLevelIncreaseFunction(level);
    }

    private void OnLevelSync(int oldLevel, int newLevel)
        => OnLevelChanged?.Invoke(newLevel);

    private void OnXpPerLevelSync(int oldXpPerLevel, int newXpPerLevel)
        => OnValueChanged?.Invoke();
}