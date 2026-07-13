using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusEffectHandler : NetworkBehaviour
{
    private readonly Dictionary<Type, StatusEffect> _effects = new();

    private void Update()
    {
        if (_effects.Count == 0)
            return;

        foreach (var effect in _effects.Values)
        {
            effect.Tick(Time.deltaTime);

            if (effect.Accumulation <= 0f)
                RemoveEffect(effect);
        }
    }

    public void Cleanse(StatusEffect effect)
    {
        if (_effects.TryGetValue(effect.GetType(), out StatusEffect eff))
            RemoveEffect(eff);
    }

    public void TryProc(StatusEffect effect, float amount, Entity target)
    {
        StatusEffect eff = GetOrAdd(effect);

        if (eff is ProlongedStatusEffect prolonged && prolonged.Active)
            return;

        if (eff.Accumulate(amount))
        {
            eff.Proc(target);

            if (eff is InstantStatusEffect)
                RemoveEffect(effect);
        }
    }

    private StatusEffect GetOrAdd(StatusEffect effect)
    {
        Type type = effect.GetType();

        if (!_effects.ContainsKey(type))
            _effects.Add(type, effect);

        return _effects[type];
    }

    private void RemoveEffect(StatusEffect effect)
    {
        if (_effects.TryGetValue(effect.GetType(), out StatusEffect eff))
        {
            eff.Reset();
            _effects.Remove(effect.GetType());
        }
    }

    public override string ToString()
        => string.Join("\n", _effects.Values);
}