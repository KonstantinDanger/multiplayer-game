using System;
using UnityEngine;

public class Level : MonoBehaviour, IGauge
{
    public event Action OnValueChanged;
    public Action<Level> OnLevelChanged;
    public Action<float> OnXpReceived;

    public int Lvl { get; private set; }
    public int MaxLvl { get; private set; }
    public float Xp { get; private set; }
    public float XpPerLevel { get; private set; }

    public float CurrentGaugeValue => Xp;
    public float MaxGaugeValue => XpPerLevel;

    private Func<int, float> _xpPerLevelIncreaseFunction;

    public void Initialize(Func<int, float> xpIncreaseFunc = null, int level = 1, int maxLevel = 10, float xp = 0)
    {
        Lvl = level;
        MaxLvl = maxLevel;
        Xp = xp;

        if (xpIncreaseFunc == null)
            xpIncreaseFunc = Utils.DefaultXpIncreaseFormula;

        _xpPerLevelIncreaseFunction = xpIncreaseFunc;

        XpPerLevel = GetNextXpPerLevel(Lvl);
    }

    public void AddXp(float amount)
    {
        if (Lvl >= MaxLvl)
            return;

        if (amount <= 0)
            return;

        Xp += amount;

        if (Xp >= XpPerLevel)
        {
            float xpRemainder = Xp - XpPerLevel;
            LevelUp(xpRemainder);
        }

        OnValueChanged?.Invoke();
    }

    private void LevelUp(float xpRemainder)
    {
        Xp = xpRemainder;

        Lvl++;

        XpPerLevel = GetNextXpPerLevel(Lvl);
    }

    private float GetNextXpPerLevel(int level)
        => _xpPerLevelIncreaseFunction(level);
}