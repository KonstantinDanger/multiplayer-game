using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Attack
{
    [SerializeField, Range(1, 1000)] protected int Amount = 1;
    [SerializeField, Range(0, 5)] protected float TimeBetweenAttacks = 0;
    [SerializeField] public Damage Damage;

    [Header("Target detection")]
    [SerializeReference, SubclassSelector] private TargetDetector _targetDetector;

    private bool _inProcess;
    public bool InProcess => _inProcess;
    public float AttackRange => Damage.Range;
    public float TimeBetweenAttack => TimeBetweenAttacks;

    public IEnumerator Apply(NetworkBehaviour sender)
    {
        if (_inProcess)
            yield break;

        if (sender != null)
            Damage.SenderNetId = sender.netId;

        if (sender.TryGetComponent(out Entity entity))
            Damage.TeamId = entity.TeamId;

        sender.TryGetComponent(out IAttacker attacker);
        sender.TryGetComponent(out IRotatable rotatable);

        _targetDetector.DetectAllTargets(
            sender.gameObject,
            attacker.AttackPoint.position,
            rotatable.Forward,
            Damage.Range,
            out List<GameObject> targets);

        if (RequireAlternatingAttacks())
        {
            yield return AlternateAttacksRoutine(sender, targets);
        }

        for (int i = 0; i < Amount; i++)
        {
            for (int j = 0; j < targets.Count; j++)
            {
                yield return OnApply(sender, targets[j]);
            }
        }
    }

    protected abstract IEnumerator OnApply(NetworkBehaviour sender, GameObject target);

    private bool RequireAlternatingAttacks()
        => Amount > 1 && TimeBetweenAttacks > 0f;

    private IEnumerator AlternateAttacksRoutine(NetworkBehaviour sender, List<GameObject> targets)
    {
        _inProcess = true;

        int currentAttackNum = 1;
        float elapsedAttackTime = TimeBetweenAttacks;

        while (currentAttackNum <= Amount)
        {
            elapsedAttackTime += Time.deltaTime;

            if (elapsedAttackTime >= TimeBetweenAttacks)
            {
                for (int j = 0; j < targets.Count; j++)
                {
                    yield return OnApply(sender, targets[j]);
                }

                elapsedAttackTime = 0f;
                currentAttackNum++;
            }

            yield return null;
        }

        _inProcess = false;
    }
}

