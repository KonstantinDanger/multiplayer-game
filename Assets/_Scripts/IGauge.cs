using System;

public interface IGauge
{
    event Action OnValueChanged;
    float CurrentValue { get; }
    float MaxValue { get; }
}

