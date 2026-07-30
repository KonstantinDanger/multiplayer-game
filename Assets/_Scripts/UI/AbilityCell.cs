using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;

    private AbilitySlot _abilitySlot;

    public void SetAbility(AbilitySlot abilityHandler, IAbilityPresentationData abilityData)
    {
        _abilitySlot = abilityHandler;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _rechargeGauge.Initialize(_abilitySlot);
    }
}
