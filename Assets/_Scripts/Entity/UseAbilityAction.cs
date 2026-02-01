using Mirror;
using System;
using UnityEngine;

[Serializable]
public class UseAbilityAction : AIAction
{
    [SerializeField] private ScriptableAbility _ability;

    private AbilityHandler _abilityHandler;

    public override void Initialize(NetworkBehaviour self)
        => _abilityHandler = new AbilityHandler(_ability.GetNew());

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        if (!_abilityHandler.IsReadyToUse())
            return;

        _abilityHandler.Perform(self, target);
    }
}
