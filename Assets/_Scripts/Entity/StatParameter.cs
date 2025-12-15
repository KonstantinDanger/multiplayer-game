using UnityEngine;

[System.Serializable]
public class StatParameter
{
    public StatParameter(StatType stat, float value)
    {
        Stat = stat;
        Value = value;
    }

    [field: SerializeField] public StatType Stat { get; private set; }
    [field: SerializeField] public float Value { get; private set; }

    public StatParameter Clone()
        => new StatParameter(Stat, Value);
}
