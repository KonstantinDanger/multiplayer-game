using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;
    [SerializeField] private GaugeBar _accumulationGauge;

    private IAbilityUser _abilityUser;

    private bool _isSet;

    public void SetAbility(IAbilityPresentationData abilityData, IAbilityUser abilityUser)
    {
        _abilityUser = abilityUser;

        if (_isSet)
            abilityUser.OnAbilitySlotStateChange -= HandleSlotStateChange;

        abilityUser.OnAbilitySlotStateChange += HandleSlotStateChange;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _isSet = true;
    }

    private void OnDestroy()
        => _abilityUser.OnAbilitySlotStateChange -= HandleSlotStateChange;

    private void HandleSlotStateChange(AbilitySlotData data)
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
