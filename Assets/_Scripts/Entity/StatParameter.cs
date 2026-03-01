using UnityEngine;

[System.Serializable]
public struct StatParameter
{
    public StatParameter(StatType stat, float value)
    {
        Stat = stat;
        Value = value;
    }

    [field: SerializeField] public StatType Stat { get; private set; }
    [field: SerializeField, Range(0f, 1000f)] public float Value { get; set; }

    public readonly StatParameter Clone()
        => new StatParameter(Stat, Value);
}
