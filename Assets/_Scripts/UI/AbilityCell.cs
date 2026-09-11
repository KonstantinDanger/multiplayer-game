using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;
    [SerializeField] private GaugeBar _accumulationGauge;

    private IAbilityUser _abilityUser;

    private bool _isSet;
    private int _slotIndex;

    public void SetAbility(int slotIndex, IAbilityPresentationData abilityData, IAbilityUser abilityUser)
    {
        _slotIndex = slotIndex;
        _abilityUser = abilityUser;

        if (_isSet)
            abilityUser.RpcOnAbilitySlotStateChange -= HandleSlotStateChange;

        abilityUser.RpcOnAbilitySlotStateChange += HandleSlotStateChange;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _isSet = true;
    }

    private void OnDestroy()
        => _abilityUser.RpcOnAbilitySlotStateChange -= HandleSlotStateChange;

    private void HandleSlotStateChange(int slotIndex, AbilitySlotData data)
    {
        if (_slotIndex != slotIndex)
            return;

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
