using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;
    [SerializeField] private GaugeBar _accumulationGauge;

    private AbilitySlot _abilitySlot;

    public void SetAbility(AbilitySlot slot, Ability ability, IAbilityPresentationData abilityData, bool cumulative)
    {
        _abilitySlot = slot;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _rechargeGauge.Initialize(_abilitySlot);

        _accumulationGauge.gameObject.SetActive(cumulative);

        if (cumulative)
            _accumulationGauge.Initialize(ability as CumulativeAbility);
    }
}
