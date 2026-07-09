using System.Collections.Generic;
using UnityEngine;

public class AbilitiesHUD : HUD
{
    [SerializeField] private List<AbilityCell> _abilityCells = new();

    public void Initialize(AbilityUser abilities)
    {
        for (int i = 0; i < _abilityCells.Count; i++)
        {
            var abilityCell = _abilityCells[i];
            var abilityHandler = abilities.Slots[i + 1];

            abilityCell.SetAbility(abilityHandler);
        }
    }
}
