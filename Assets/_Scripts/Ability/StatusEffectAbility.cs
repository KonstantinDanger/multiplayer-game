using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusEffectAbility : Ability
{
    [SerializeField] private ScriptableStatusEffect _effect;
    [SerializeField] private bool _procGuarantee;
    [Header("if proc guarantee option is selected accumulation amount won't be considered")]
    [SerializeField, Range(0f, 10000f)] private float _accumulationAmount;
    [SerializeField, Range(0f, 10000f)] private float _detectionRange;
    [SerializeField] private bool _procOnSelfOnly = true;
    [SerializeReference, SubclassSelector] private TargetDetector _targetDetector;

    protected sealed override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        sender.TryGetComponent(out IAttacker attacker);
        sender.TryGetComponent(out IRotatable rotatable);

        _targetDetector.DetectAllTargets(
            sender.gameObject,
            attacker.AttackPoint.position,
            rotatable.Forward,
            _detectionRange,
            out List<GameObject> targets);

        foreach (GameObject item in targets)
        {
            if (_procOnSelfOnly && item != sender.gameObject)
                continue;

            TryProc(item, _effect);
        }

        yield return null;
    }

    private void TryProc(GameObject target, ScriptableStatusEffect effect)
    {
        if (target.TryGetComponent(out StatusEffectHandler effectsHandler))
        {
            effectsHandler.TryProc(effect, _accumulationAmount, _procGuarantee);
        }
    }
}
