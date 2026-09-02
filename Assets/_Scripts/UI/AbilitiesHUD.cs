using System.Collections.Generic;
using UnityEngine;

public class AbilitiesHUD : HUD
{
    [SerializeField] private List<AbilityCell> _abilityCells = new();

    public void Initialize(AbilityUser abilityUser)
    {
        for (int i = 0; i < _abilityCells.Count; i++)
        {
            AbilityCell abilityCell = _abilityCells[i];
            AbilitySlot abilitySlot = abilityUser.Slots[i + 1];
            abilityUser.GetPresentationData(abilitySlot);
            IAbilityPresentationData data = abilitySlot.AbilityInstance.presentationData;

            abilityCell.SetAbility(abilitySlot, data);
        }
    }
}
