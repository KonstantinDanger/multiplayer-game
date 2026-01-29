using System.Collections.Generic;
using UnityEngine;

public class AbilitiesHUD : MonoBehaviour
{
    [SerializeField] private List<AbilityCell> _abilityCells = new();

    public void Initialize(PlayerAbilityUser abilities)
    {
        for (int i = 0; i < _abilityCells.Count; i++)
        {
            var abilityCell = _abilityCells[i];
            var abilityHandler = abilities.Handlers[i + 1];

            abilityCell.SetAbility(abilityHandler);
        }
    }
}
