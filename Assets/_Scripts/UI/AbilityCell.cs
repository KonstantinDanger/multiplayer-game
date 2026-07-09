using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;

    private AbilitySlot _abilitySlot;

    public void SetAbility(AbilitySlot abilityHandler)
    {
        _abilitySlot = abilityHandler;

        _rechargeGauge.Initialize(_abilitySlot);
    }
}
