using System;

public interface IGauge
{
    event Action OnValueChanged;
    float CurrentGaugeValue { get; }
    float MaxGaugeValue { get; }
}

