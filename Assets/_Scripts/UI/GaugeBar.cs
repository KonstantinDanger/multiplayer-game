using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private bool _reverseGauge;

    private IGauge Gauge { get; set; }

    private bool _initialized;

    private void OnEnable()
    {
        if (Gauge != null)
            HandleValueChanged();
    }

    public void Initialize(IGauge gauge)
    {
        if (_initialized)
            return;

        Gauge = gauge;

        Gauge.OnValueChanged += HandleValueChanged;

        _initialized = true;
    }

    private void OnDestroy()
        => Gauge.OnValueChanged -= HandleValueChanged;

    private void HandleValueChanged()
    {
        if (Gauge is CumulativeAbility)
            UnityEngine.Debug.Log("asdasdasdas ");

        float value = Gauge.CurrentGaugeValue / Gauge.MaxGaugeValue;
        value = Mathf.Clamp01(value);

        if (_reverseGauge)
            value = 1f - value;

        SetSliderValue(value);
    }

    private void SetSliderValue(float value)
        => _slider.value = value;
}

