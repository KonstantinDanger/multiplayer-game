using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CompoundAbility : Ability, ICacheAbilities
{
    [SerializeField] private List<ScriptableAbility> _abilities;

    private List<AbilityInstance> _cached;

    public IEnumerable<AbilityInstance> CacheAbilities()
    {
        _cached = _abilities
            .Select(ability => new AbilityInstance(ability.GetNew(), ability))
            //.Concat(_abilities
            //.OfType<ICacheAbilities>()
            //.SelectMany(ability => ability.CacheAbilities())) ///If compound ability can have compound abilities within itself
            .ToList();

        return _cached;
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        foreach (AbilityInstance instance in _cached)
        {
            instance.ability.Perform(sender, target);
        }

        while (_cached.Any(instance => instance.ability.IsPerforming))
        {
            yield return null;
        }
    }
}
