using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Attack
{
    [SerializeField, Range(1, 1000)] protected int Amount = 1;
    [SerializeField, Range(0, 5)] protected float TimeBetweenAttacks = 0;
    [SerializeField] protected Damage Damage;

    private bool _inProcess;

    public void Apply(GameObject sender, GameObject target)
    {
        if (_inProcess)
            return;

        var coroutineHolder = ServiceLocator.Container.Resolve<CoroutineHolder>();

        if (RequireAlternatingAttacks())
        {
            coroutineHolder.StartCoroutine(AlternateAttacksRoutine(sender, target));
            return;
        }

        for (int i = 0; i < Amount; i++)
            OnApply(sender, target);
    }


    protected abstract void OnApply(GameObject sender, GameObject target);

    private bool RequireAlternatingAttacks()
        => Amount > 1 && TimeBetweenAttacks > 0f;

    private IEnumerator AlternateAttacksRoutine(GameObject sender, GameObject target)
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

