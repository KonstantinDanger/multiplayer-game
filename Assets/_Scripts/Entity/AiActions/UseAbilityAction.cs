using Mirror;
using System;
using UnityEngine;

[Serializable]
public class UseAbilityAction : AIAction
{
    [SerializeField] private ScriptableAbility _ability;

    private IAbilityUser _abilityUser;

    private Ability _selectedAbility;

    public override void Initialize(NetworkBehaviour self)
    {
        _abilityUser = self.GetComponent<IAbilityUser>();
        _abilityUser.Add(_ability.GetNew());
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        IsLocked = true;
        _selectedAbility = _abilityUser.Use(target);

        if (_selectedAbility == null)
        {
            IsLocked = false;
            return;
        }

        IsLocked = _selectedAbility.IsPerforming;
    }
}
