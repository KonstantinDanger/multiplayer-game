using Mirror;
using System;
using System.Collections.Generic;

public class AbilityUser : NetworkBehaviour, IAbilityUser
{
    private readonly List<AbilityHandler> _abilities = new();

    public IReadOnlyList<AbilityHandler> Handlers => _abilities;

    public event Action<UseAbilityData> OnAbilityStartUsing;

    public void Initialize(List<Ability> abilities)
        => abilities.ForEach(ability => Add(ability));

    public void OnUpdate()
    {
        foreach (var ability in _abilities)
            ability?.Update();
    }

    public void Add(Ability ability)
        => _abilities.Add(new(ability));

    /// <summary>
    /// Method for calling primary ability (0 index)
    /// </summary>
    public virtual Ability Use(NetworkBehaviour target = null)
    {
        AbilityHandler ability = _abilities[0];

        if (ability.Perform(this, target))
        {
            UseAbilityData data = new UseAbilityData()
            {
                UsagePreparationAnimDuration = ability.PreparationAnimation.averageDuration,
                PreparationAnimationName = ability.PreparationAnimation.name,
                UsagePreparationTime = ability.UsagePrepareTime,

                UsageAnimationName = ability.UsageAnimation.name,
            };

            OnAbilityStartUsing.Invoke(data);
        }

        return ability;
    }

    public virtual Ability Use(int index, NetworkBehaviour target = null)
    {
        if (index > _abilities.Count)
            throw new System.Exception("Wrong index");

        AbilityHandler selected = _abilities[index];

        if (selected == null)
            return null;

        selected.Perform(this, target);

        return selected;
    }
}
