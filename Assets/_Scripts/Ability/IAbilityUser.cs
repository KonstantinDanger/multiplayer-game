using Mirror;
using System;
using System.Collections.Generic;

public interface IAbilityUser
{
    event Action<UseAbilityData> OnAbilityStartUsing;

    IReadOnlyList<AbilityHandler> Handlers { get; }

    void Initialize(List<Ability> abilities);
    void OnUpdate();
    //Ability Use(Ability abilityToUse, NetworkBehaviour target = null);
    Ability Use(int index, NetworkBehaviour target = null);
    void Add(Ability ability);
    int FindIndexOf(Ability abilityToUse);
}
