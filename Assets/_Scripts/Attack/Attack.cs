using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Attack
{
    [SerializeField, Range(1, 1000)] protected int Amount = 1;
    [SerializeField, Range(0, 5)] protected float TimeBetweenAttacks = 0;
    [SerializeField] private Damage _damage;

    private bool _inProcess;
    public bool InProcess => _inProcess;
    public Damage Damage => _damage;
    public float AttackRange => Damage.Range;

    public void Apply(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_inProcess)
            return;

        if (sender != null)
            _damage.SenderNetId = sender.netId;

        if (target != null)
            _damage.ReceiverNetId = target.netId;

        var coroutineHolder = ServiceLocator.Container.Resolve<CoroutineHolder>();

        if (RequireAlternatingAttacks())
        {
            coroutineHolder.StartCoroutine(AlternateAttacksRoutine(sender, target));
            return;
        }

        for (int i = 0; i < Amount; i++)
            OnApply(sender, target);
    }


    protected abstract void OnApply(NetworkBehaviour sender, NetworkBehaviour target);

    private bool RequireAlternatingAttacks()
        => Amount > 1 && TimeBetweenAttacks > 0f;

    private IEnumerator AlternateAttacksRoutine(NetworkBehaviour sender, NetworkBehaviour target)
    {
        _inProcess = true;

        int currentAttackNum = 1;
        float elapsedAttackTime = TimeBetweenAttacks;

        while (currentAttackNum <= Amount)
        {
            elapsedAttackTime += Time.deltaTime;

            if (elapsedAttackTime >= TimeBetweenAttacks)
            {
                OnApply(sender, target);
                elapsedAttackTime = 0f;
                currentAttackNum++;
            }

            yield return null;
        }

        _inProcess = false;
    }
}

