using Mirror;
using System;

public class Level : NetworkBehaviour, IGauge
{
    public event Action OnValueChanged;
    public Action<int> OnLevelChanged;
    public Action<float> OnXpReceived;

    [SyncVar(hook = nameof(OnLevelSync))] private int _level;
    [SyncVar] private int _maxLevel;
    [SyncVar(hook = nameof(OnXpSync))] private float _xp;
    [SyncVar(hook = nameof(OnXpPerLevelSync))] private float _xpPerLevel;

    public int Lvl => _level;
    public int MaxLvl => _maxLevel;
    public float Xp => _xp;
    public float XpPerLevel => _xpPerLevel;
    public float CurrentGaugeValue => Xp;
    public float MaxGaugeValue => XpPerLevel;

    private Func<int, float> _xpPerLevelIncreaseFunction;

    [Server]
    public void Initialize(Func<int, float> xpIncreaseFunc = null, int level = 1, int maxLevel = 10, float xp = 0)
    {
        _level = level;
        _maxLevel = maxLevel;
        _xp = xp;

        if (xpIncreaseFunc != null)
            _xpPerLevelIncreaseFunction = xpIncreaseFunc;

        _xpPerLevel = GetNextXpPerLevel(_level);

        OnLevelChanged?.Invoke(_level);
        OnXpReceived?.Invoke(_xp);
        OnValueChanged?.Invoke();
    }

    [Command(requiresAuthority = false)]
    public void AddXp(float amount)
    {
        if (Lvl >= MaxLvl)
            return;

        if (amount <= 0)
            return;

        _xp += amount;

        if (Xp >= XpPerLevel)
        {
            float xpRemainder = Xp - XpPerLevel;
            LevelUp(xpRemainder);
        }
    }

    [Server]
    private void LevelUp(float xpRemainder)
    {
        _xp = xpRemainder;
        _level++;
        _xpPerLevel = GetNextXpPerLevel(Lvl);
    }

    private float GetNextXpPerLevel(int level)
    {
        if (_xpPerLevelIncreaseFunction == null)
            _xpPerLevelIncreaseFunction = Utils.DefaultXpIncreaseFormula;

        return _xpPerLevelIncreaseFunction(level);
    }

    private void OnLevelSync(int oldLevel, int newLevel)
        => OnLevelChanged?.Invoke(newLevel);

    private void OnXpPerLevelSync(float oldXpPerLevel, float newXpPerLevel)
        => OnValueChanged?.Invoke();

    private void OnXpSync(float oldXp, float newXp)
    {
        OnXpReceived?.Invoke(newXp - oldXp);
        OnValueChanged?.Invoke();
    }
}