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
            AbilitySlot abilitySlot = abilityUser.Slots[i + 1];

            AbilityInstance currentInstance = abilitySlot.AbilityInstance;
            Ability outputAbility = currentInstance.ability;

            IAbilityPresentationData data = currentInstance.presentationData;
            bool isCumulative = false;

            if (currentInstance.ability is IPresentInnerAbility innerPresenter)
            {
                AbilityInstance innerInstance = innerPresenter.GetInnerAbilityInstance(owner);
                IAbilityPresentationData presentationData = innerInstance.presentationData;
                outputAbility = innerInstance.ability;

                isCumulative = innerInstance.ability is CumulativeAbility;

                if (presentationData != null)
                {
                    data = presentationData;
                }
            }

            //IAbilityPresentationData data = abilityUser.GetPresentationData(abilitySlot);
            //IAbilityPresentationData data = abilitySlot.AbilityInstance.presentationData;

            abilityCell.SetAbility(abilitySlot, outputAbility, data, isCumulative);
        }
    }
}
