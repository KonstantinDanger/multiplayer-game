using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;

    private AbilityHandler _abilityHandler;

    public void SetAbility(AbilityHandler abilityHandler)
    {
        _abilityHandler = abilityHandler;

        _rechargeGauge.Initialize(_abilityHandler);
    }
}
