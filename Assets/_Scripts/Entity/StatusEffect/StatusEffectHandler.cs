using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusEffectHandler : NetworkBehaviour
{
    public event Action<IStatusEffectPresentationData> OnInstantEffectProc;
    public event Action<IStatusEffectPresentationData> OnProlongedEffectProc;
    public event Action<IStatusEffectPresentationData> OnProlongedEffectRemoved;

    private readonly Dictionary<ScriptableStatusEffect, StatusEffectInstance> _effects = new();

    private void Update()
    {
        if (_effects.Count == 0)
            return;

        foreach (var pair in _effects.ToList())
        {
            StatusEffect effect = pair.Value.Effect;

            effect.Tick(Time.deltaTime);

            if (effect.Accumulation <= 0f)
                RemoveEffect(pair.Key);
        }
    }

    public void Cleanse(ScriptableStatusEffect effect)
    {
        if (_effects.ContainsKey(effect))
            RemoveEffect(effect);
    }

    public void CleanseAll()
    {
        foreach (var item in _effects)
            item.Value.Effect.Reset();

        _effects.Clear();
    }

    public void TryProc(ScriptableStatusEffect effect, float accumulationAmount, bool procGuarantee = false)
    {
        StatusEffect eff = GetOrAdd(effect);

        if (eff is ProlongedStatusEffect prolonged && prolonged.Active)
            return;

        if (procGuarantee)
            accumulationAmount = eff.MaxAccumulationAmount;

        if (eff.Accumulate(accumulationAmount))
        {
            eff.Proc(gameObject);

            if (eff is InstantStatusEffect)
            {
                OnInstantEffectProc?.Invoke(effect);
                RemoveEffect(effect);
            }
            else
            {
                OnProlongedEffectProc?.Invoke(effect);
            }
        }
    }

    private StatusEffect GetOrAdd(ScriptableStatusEffect effect)
    {
        if (!_effects.ContainsKey(effect))
            _effects.Add(effect, new StatusEffectInstance(effect, effect.GetNew()));

        return _effects[effect].Effect;
    }

    private void RemoveEffect(ScriptableStatusEffect effect)
    {
        if (_effects.TryGetValue(effect, out StatusEffectInstance instance))
        {
            instance.Effect.Reset();
            _effects.Remove(effect);

            if (instance.Effect is ProlongedStatusEffect)
                OnProlongedEffectRemoved?.Invoke(effect);
        }
    }

    public override string ToString()
        => string.Join("\n", _effects.Values);
}