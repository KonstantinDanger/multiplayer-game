using System;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;
    [SerializeField] private GaugeBar _accumulationGauge;

    private event Action<int, AbilitySlotData> OnAbilitySlotStateChange;

    private bool _isSet;

    public void SetAbility(IAbilityPresentationData abilityData, Action<int, AbilitySlotData> onAbilitySlotStateChange)
    {
        if (_isSet)
            OnAbilitySlotStateChange -= HandleSlotStateChange;

        OnAbilitySlotStateChange = onAbilitySlotStateChange;

        OnAbilitySlotStateChange += HandleSlotStateChange;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _isSet = true;
    }

    private void HandleSlotStateChange(int index, AbilitySlotData data)
    {
        _rechargeGauge.SetValue(data.RechargeProgress);

        if (data.MaxCharges == 0)
        {
            _accumulationGauge.gameObject.SetActive(false);
            return;
        }

        _accumulationGauge.gameObject.SetActive(true);
        _accumulationGauge.SetValue((float)data.AccumulatedCharges / data.MaxCharges);
    }
}
