using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    private readonly List<AbilityHandler> _abilities = new();

    public void Initialize(List<Ability> abilities)
        => abilities
        .ForEach(ability => _abilities
        .Add(new AbilityHandler(ability)));

    public void Use(int index, GameObject target = null)
    {
        if (index >= _abilities.Count)
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
