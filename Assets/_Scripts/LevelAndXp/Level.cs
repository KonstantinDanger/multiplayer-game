using Mirror;
using System;

public class Level : NetworkBehaviour, IGauge
{
    public event Action OnValueChanged;
    public Action<int> OnLevelChanged;
    public Action<float> OnXpReceived;

    [SyncVar] private int _level;
    [SyncVar] private int _maxLevel;
    [SyncVar] private float _xp;

    public int Lvl => _level;
    public int MaxLvl => _maxLevel;
    public float Xp => _xp;
    public float XpPerLevel { get; private set; }

    public float CurrentGaugeValue => Xp;
    public float MaxGaugeValue => XpPerLevel;

    private Func<int, float> _xpPerLevelIncreaseFunction;

    public void Initialize(Func<int, float> xpIncreaseFunc = null, int level = 1, int maxLevel = 10, float xp = 0)
    {
        _level = level;
        _maxLevel = maxLevel;
        _xp = xp;

        if (xpIncreaseFunc == null)
            xpIncreaseFunc = Utils.DefaultXpIncreaseFormula;

        _xpPerLevelIncreaseFunction = xpIncreaseFunc;

        XpPerLevel = GetNextXpPerLevel(Lvl);

        RpcOnLevelChanged(_level);
        RpcOnXpReceived(_xp);
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

        RpcOnXpReceived(amount);
        RpcOnValueChanged();
    }

    [Server]
    private void LevelUp(float xpRemainder)
    {
        _xp = xpRemainder;

        _level++;

        RpcOnLevelChanged(_level);

        XpPerLevel = GetNextXpPerLevel(Lvl);
    }

    private float GetNextXpPerLevel(int level)
        => _xpPerLevelIncreaseFunction(level);

    [ClientRpc] private void RpcOnValueChanged() => OnValueChanged?.Invoke();
    [ClientRpc] private void RpcOnLevelChanged(int level) => OnLevelChanged?.Invoke(level);
    [ClientRpc] private void RpcOnXpReceived(float amount) => OnXpReceived?.Invoke(amount);
}