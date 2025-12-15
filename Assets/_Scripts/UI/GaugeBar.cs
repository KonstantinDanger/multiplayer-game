using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : NetworkBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private bool _reverseGauge;

    private IGauge Gauge { get; set; }

    private bool _initialized;

    public void Initialize(IGauge gauge)
    {
        if (_initialized)
            return;

        Gauge = gauge;

        Gauge.OnValueChanged += HandleValueChanged;

        _initialized = true;

        HandleValueChanged();
    }

    private void OnEnable()
    {
        if (Gauge == null)
            return;

        Gauge.OnValueChanged += HandleValueChanged;
    }

    private void OnDisable()
        => Gauge.OnValueChanged -= HandleValueChanged;

    private void HandleValueChanged()
    {
        float value = Gauge.CurrentGaugeValue / Gauge.MaxGaugeValue;
        value = Mathf.Clamp01(value);

        if (_reverseGauge)
            value = 1f - value;

        SetSliderValue(value);
    }

    private void SetSliderValue(float value)
        => _slider.value = value;
}

