using System;

public class CumulativeAbilityGauge : IGauge
{
    private readonly CumulativeAbility _ability;

    private int _accumulationDelta;

    public CumulativeAbilityGauge(CumulativeAbility cumulativeAbility)
        => _ability = cumulativeAbility;

    public float CurrentGaugeValue
    {
        get
        {
            if (_accumulationDelta != _ability.AccumulatedCharges)
            {
                OnValueChanged?.Invoke();
            }

            _accumulationDelta = _ability.AccumulatedCharges;

            return _ability.AccumulatedCharges;
        }
    }

    public float MaxGaugeValue => _ability.MaxCharges;

    public event Action OnValueChanged;
}

