using Mirror;
using System;
using System.Collections.Generic;

public class AbilityUser : NetworkBehaviour, IAbilityUser
{
    private readonly List<AbilityHandler> _abilities = new();

    public IReadOnlyList<AbilityHandler> Handlers => _abilities;

    public event Action<UseAbilityData> OnAbilityStartUsing;

    public void Initialize(List<Ability> abilities)
    {
        if (_abilities.Count > 0)
            _abilities.Clear();

        abilities.ForEach(ability => Add(ability));
    }

    public void OnUpdate()
    {
        foreach (var ability in _abilities)
            ability?.Update();
    }

    public void Add(Ability ability)
        => _abilities.Add(new(ability));

    public virtual Ability Use(Ability abilityToUse, NetworkBehaviour target = null)
    {
        AbilityHandler ability = _abilities.Find(handler => handler.Ability.Equals(abilityToUse));

        if (ability.Perform(this, target))
            OnAbilityStartUsing.Invoke(SetupData(ability));

        return ability;
    }

    public virtual Ability Use(int index, NetworkBehaviour target = null)
    {
        if (index > _abilities.Count)
            throw new System.Exception("Wrong index");

        AbilityHandler selected = _abilities[index];

        if (selected == null)
            return null;

        if (selected.Perform(this, target))
            OnAbilityStartUsing.Invoke(SetupData(selected));

        return selected;
    }

    private UseAbilityData SetupData(Ability ability)
    {
        return new UseAbilityData()
        {
            UsagePreparationAnimDuration = ability.PreparationAnimation.averageDuration,
            PreparationAnimationName = ability.PreparationAnimation.name,
            UsagePreparationTime = ability.UsagePrepareTime,

            UsageAnimationName = ability.UsageAnimation.name,
        };
    }
}
