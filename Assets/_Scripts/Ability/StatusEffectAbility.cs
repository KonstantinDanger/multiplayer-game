using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class StatusEffectAbility : Ability
{
    [SerializeField] private ScriptableStatusEffect _effect;

    protected sealed override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!sender.TryGetComponent(out Entity entity))
            yield break;

        _effect
            .GetNew()
            .Proc(entity);

        yield return null;
    }
}
