public class AbilityInstance
{
    public readonly Ability ability;
    public readonly IAbilityPresentationData presentationData;

    public AbilityInstance(Ability ability, IAbilityPresentationData presentationData)
    {
        this.ability = ability;
        this.presentationData = presentationData;
    }

    public AbilityInstance(AbilityInstance instance)
    {
        ability = instance.ability;
        presentationData = instance.presentationData;
    }
}
