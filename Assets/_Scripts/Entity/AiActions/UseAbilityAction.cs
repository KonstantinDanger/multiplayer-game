using Mirror;
using System;
using UnityEngine;

[Serializable]
public class UseAbilityAction : AIAction
{
    [SerializeField] private ScriptableAbility _ability;

    private IAbilityUser _abilityUser;

    public override void Initialize(NetworkBehaviour self)
    {
        _abilityUser = self.GetComponent<IAbilityUser>();
        _abilityUser.Add(_ability.GetNew());
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
        => _abilityUser.Use(target);
}
