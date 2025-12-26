using Mirror;
using System.Collections.Generic;

public interface IAbilityUser
{
    IReadOnlyList<AbilityHandler> Handlers { get; }

    void Initialize(List<Ability> abilities);
    void OnUpdate();
    void Use(NetworkBehaviour target = null);
    void Use(int index, NetworkBehaviour target = null);
}
