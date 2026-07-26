using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusEffectHandler : NetworkBehaviour
{
    private readonly Dictionary<Type, StatusEffect> _effects = new();

    private void Update()
    {
        if (_effects.Count == 0)
            return;

        foreach (var effect in _effects.Values.ToList())
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

    public void TryProc(StatusEffect effect, float amount)
    {
        StatusEffect eff = GetOrAdd(effect);

        if (eff is ProlongedStatusEffect prolonged && prolonged.Active)
            return;

        if (eff.Accumulate(amount))
        {
            eff.Proc(gameObject);

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
        Type type = effect.GetType();

        if (_effects.TryGetValue(type, out StatusEffect eff))
        {
            eff.Reset();
            _effects.Remove(type);
        }
    }

    public override string ToString()
        => string.Join("\n", _effects.Values);
}