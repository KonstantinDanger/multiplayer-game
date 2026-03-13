using Mirror;
using System;
using System.Collections.Generic;

public class PlayerAbilityUser : NetworkBehaviour, IAbilityUser
{
    private readonly List<AbilityHandler> _abilities = new();

    public IReadOnlyList<AbilityHandler> Handlers => _abilities;

    public event Action<UseAbilityData> OnAbilityStartUsing;

    public void Initialize(List<Ability> abilities)
    {
        abilities
            .ForEach(ability => _abilities
            .Add(new AbilityHandler(ability)));
    }

    public void OnUpdate()
    {
        foreach (var ability in _abilities)
            ability?.Update();
    }

    /// <summary>
    /// Method for calling primary ability (0 index)
    /// </summary>
    public virtual void Use(NetworkBehaviour target = null)
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
    }

    public virtual void Use(int index, NetworkBehaviour target = null)
    {
        if (index > _abilities.Count)
            throw new System.Exception("Wrong index");

        AbilityHandler selected = _abilities[index];

        if (selected == null)
            return;

        selected.Perform(this, target);
    }
}
