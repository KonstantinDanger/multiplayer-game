using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;
    [SerializeField] private GaugeBar _accumulationGauge;

    private AbilitySlot _abilitySlot;

    public void SetAbility(AbilitySlot slot, IAbilityPresentationData abilityData)
    {
        _abilitySlot = slot;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _rechargeGauge.Initialize(_abilitySlot);

        if (_abilitySlot.AbilityInstance.ability is not CumulativeAbility cumulativeAbility)
        {
            _accumulationGauge.gameObject.SetActive(false);
            return;
        }

        _accumulationGauge.gameObject.SetActive(true);

        _accumulationGauge.Initialize(cumulativeAbility);
    }
}
