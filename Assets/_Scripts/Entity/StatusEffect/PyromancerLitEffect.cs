using Mono.CecilX;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PyromancerLitEffect : ProlongedStatusEffect
{
    [SerializeField, Range(0f, 500f)] private float _damagePercentBuff = 15f;
    [SerializeField] private List<TypeReference/*Type of AttackAbility*/> _buffableAbilities;

    protected override void OnProc(GameObject target)
    {
        //cache entity
    }

    protected override void OnTick(float deltaTime)
    {
        //while the effect is active ->
        //var nextUsedAbility = cachedEntity.ActiveAbilities.Find(a => _buffableAbilities.Contains(a))
        //if _buffableAbilities.Contains(nextUsedAbility.GetType()) =>
        //nextUsedAbility.Damage *= _damagePercentBuff 
    }
}
