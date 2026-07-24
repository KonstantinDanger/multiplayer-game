using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class StatusEffectAbility : Ability
{
    [SerializeField] private ScriptableStatusEffect _effect;
    [SerializeField, Range(0f, 10000f)] private float _accumulationAmount;
    [SerializeField] private bool _procOnSelf;

    protected sealed override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!_procOnSelf && sender == target)
            yield break;

        TryProc(target, _effect.GetNew());

        yield return null;
    }

    private void TryProc(NetworkBehaviour target, StatusEffect effect)
    {
        if (target.TryGetComponent(out StatusEffectHandler effectsHandler))
            effectsHandler.TryProc(effect, _accumulationAmount);
    }
}
