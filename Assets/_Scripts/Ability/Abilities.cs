using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
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
    public void Use(GameObject target = null)
        => _abilities[0].Perform(gameObject, target);

    public void Use(int index, GameObject target = null)
    {
        if (index > _abilities.Count)
            throw new System.Exception("Wrong index");

        AbilityHandler selected = _abilities[index];

        if (selected == null)
            return;


        selected.Perform(gameObject, target);
    }

    public void Upgrade(Ability ability)
    {

    }
}
