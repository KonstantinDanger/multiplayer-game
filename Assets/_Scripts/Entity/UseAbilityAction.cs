using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/UseAbilityAction", fileName = "UseAbilityAction")]
public class UseAbilityAction : AIAction
{
    [SerializeField] private ScriptableAbility _ability;

    private AbilityHandler _abilityHandler;

    public override void Initialize(NetworkBehaviour self)
        => _abilityHandler = new AbilityHandler(_ability.GetNew());

    public override void Execute(NetworkBehaviour self, NetworkBehaviour target)
    {
        if (!_abilityHandler.IsReadyToUse())
            return;

        _abilityHandler.Perform(self, target);
    }
}
