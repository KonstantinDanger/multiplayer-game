using System;

public interface IGauge
{
    /// <summary>
    /// Should be client-side!
    /// </summary>
    event Action OnValueChanged;
    float CurrentGaugeValue { get; }
    float MaxGaugeValue { get; }
}

