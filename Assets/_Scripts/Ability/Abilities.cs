using Mirror;
using System.Collections.Generic;

public class Abilities : NetworkBehaviour
{
    private readonly List<AbilityHandler> _abilities = new();

    public IReadOnlyList<AbilityHandler> Handlers => _abilities;

    public void Initialize(List<Ability> abilities)
    {
        abilities
            .ForEach(ability => _abilities
            .Add(new AbilityHandler(ability)));
    }

    private void Update()
    {
        foreach (var ability in _abilities)
            ability?.Update();
    }

    /// <summary>
    /// Method for calling primary ability (0 index)
    /// </summary>
    public void Use(NetworkBehaviour target = null)
        => _abilities[0].Perform(this, target);

    public void Use(int index, NetworkBehaviour target = null)
    {
        if (index > _abilities.Count)
            throw new System.Exception("Wrong index");

        AbilityHandler selected = _abilities[index];

        if (selected == null)
            return;


        selected.Perform(this, target);
    }

    public void Upgrade(Ability ability)
    {

    }
}
