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
            IAbilityPresentationData data = abilityUser.ScriptableAbilities[abilitySlot.Ability];

            abilityCell.SetAbility(abilitySlot, data);
        }
    }
}
