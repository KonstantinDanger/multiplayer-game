using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesHUD : HUD
{
    [SerializeField] private List<AbilityCell> _abilityCells = new();

    public void Initialize(IAbilityUser abilityUser, NetworkBehaviour owner)
    {
        for (int i = 0; i < _abilityCells.Count; i++)
        {
            AbilityCell abilityCell = _abilityCells[i];
            int slotIndex = i + 1; // + 1 because primary ability occupies the first slot
            AbilitySlot abilitySlot = abilityUser.Slots[slotIndex];

            AbilityInstance currentInstance = abilitySlot.AbilityInstance;

            IAbilityPresentationData data = currentInstance.presentationData;

            if (currentInstance.ability is IPresentInnerAbility innerPresenter)
            {
                IAbilityPresentationData presentationData = innerPresenter
                    .GetInnerAbilityInstance(owner)
                    .presentationData;

                if (presentationData != null)
                    data = presentationData;
            }

            //IAbilityPresentationData data = abilityUser.GetPresentationData(abilitySlot);
            //IAbilityPresentationData data = abilitySlot.AbilityInstance.presentationData;

            abilityCell.SetAbility(slotIndex, data, abilityUser);
        }
    }
}
