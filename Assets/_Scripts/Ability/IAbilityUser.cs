using Mirror;
using System;
using System.Collections.Generic;

public interface IAbilityUser
{
    event Action<UseAbilityData> OnAbilityStartUsing;

    IReadOnlyList<AbilityHandler> Handlers { get; }

    void Initialize(List<Ability> abilities);
    void OnUpdate();
    void Use(NetworkBehaviour target = null);
    void Use(int index, NetworkBehaviour target = null);
    void Add(Ability ability);
}
