public class AbilityInstance
{
    public readonly Ability ability;
    public readonly IAbilityPresentationData presentationData;

    private readonly ScriptableAbility _scriptableAbility;

    public AbilityInstance(Ability ability, ScriptableAbility scriptableAbility)
    {
        this.ability = ability;
        presentationData = scriptableAbility;
        _scriptableAbility = scriptableAbility;
    }

    public bool Contains(ScriptableAbility scriptableAbility)
        => scriptableAbility == _scriptableAbility;
}
