using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Ability
{
    private float _nextUsageTime = 0;

    [field: SerializeField] public virtual string Name { get; protected set; }
    [field: SerializeField] public virtual float CooldownTime { get; protected set; }
    [field: SerializeField] public virtual AnimationClip PreparationAnimation { get; protected set; }
    [field: SerializeField] public virtual AnimationClip UsageAnimation { get; protected set; }
    [field: SerializeField, Range(0f, 10f)] public virtual float UsagePrepareTime { get; protected set; } = 0.2f;

    public bool IsPerforming { get; private set; }
    public bool IsReadyToUse => Time.time >= _nextUsageTime && !IsPerforming;
    public float RechargeProgress
    {
        get
        {
            float elapsedTime = Time.time - (_nextUsageTime - CooldownTime);

            return Mathf.Clamp(elapsedTime, 0f, CooldownTime);
        }
    }

    public bool Perform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!IsReadyToUse)
            return false;

        IsPerforming = true;

        var coroutineHolder = ServiceLocator.Container.Resolve<CoroutineHolder>();
        coroutineHolder.StartCoroutine(OnPerformRoutine(sender, target));

        return true;
    }

    private IEnumerator OnPerformRoutine(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (UsagePrepareTime > 0)
            yield return PrepareUsageRoutine();

        yield return OnPerform(sender, target);

        yield return OnPerformed(sender, target);

        IsPerforming = false;
    }

    protected virtual IEnumerator OnPerformed(NetworkBehaviour sender, NetworkBehaviour target)
    {
        SetNextUseTime(CooldownTime);
        yield return null;
    }

    private IEnumerator PrepareUsageRoutine()
    {
        yield return new WaitForSeconds(UsagePrepareTime);
    }

    protected abstract IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target);

    protected void SetNextUseTime(float cooldownTime)
        => _nextUsageTime = Time.time + cooldownTime;

    public override bool Equals(object obj)
    {
        if (obj is not Ability other)
            return false;

        return other.Name == Name
            && other.CooldownTime == CooldownTime
            && other.UsagePrepareTime == UsagePrepareTime
            && other.PreparationAnimation.name == PreparationAnimation.name
            && other.UsageAnimation.name == UsageAnimation.name;
    }

    public override int GetHashCode()
        => base.GetHashCode();
}
