using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private GaugeBar _rechargeGauge;
    [SerializeField] private GaugeBar _accumulationGauge;

    private AbilitySlot _abilitySlot;

    private CumulativeAbility _cumulativeAbility;

    public void SetAbility(AbilitySlot slot, Ability ability, IAbilityPresentationData abilityData, bool cumulative)
    {
        _abilitySlot = slot;

        if (abilityData.SpriteIcon != null)
            _image.sprite = abilityData.SpriteIcon;

        _rechargeGauge.Initialize(_abilitySlot);

        _accumulationGauge.gameObject.SetActive(cumulative);


        if (cumulative)
        {
            _cumulativeAbility = ability as CumulativeAbility;
            _accumulationGauge.Initialize(_cumulativeAbility);
        }
    }

    private void Update()
        => TryUpdateCumulativeAbility(_cumulativeAbility);

    private void TryUpdateCumulativeAbility(CumulativeAbility ability)
    {
        if (ability == null)
            return;

        ability.UpdateValueChange();
    }
}
