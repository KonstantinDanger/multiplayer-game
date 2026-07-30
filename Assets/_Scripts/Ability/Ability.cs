using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class Ability
{
    public event Action<float> OnPreparationStarted;
    public event Action<float> OnPerformStarted;
    public event Action OnFinished;

    private float _nextUsageTime = 0;

    [field: SerializeField] public virtual float CooldownTime { get; protected set; }
    [field: SerializeField, Range(0f, 10f)] public virtual float UsagePrepareTime { get; protected set; } = 0.2f;

    public virtual float Duration => 0f;
    public bool IsPerforming { get; private set; }
    public bool IsRecharging => Time.time < _nextUsageTime;
    public float RechargeProgress
    {
        get
        {
            if (IsPerforming)
                return 0f;

            float elapsedTime = Time.time - (_nextUsageTime - CooldownTime);

            return Mathf.Clamp(elapsedTime, 0f, CooldownTime);
        }
    }

    public bool Perform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (IsRecharging)
            return false;

        if (IsPerforming)
        {
            switch (OnPerformRequested(sender, target))
            {
                case AbilityRequestStatus.Success:
                    break;

                case AbilityRequestStatus.InterruptWithSuccess:
                    return true;

                case AbilityRequestStatus.Deny:
                    return false;
            }
        }

        IsPerforming = true;

        var coroutineHolder = ServiceLocator.Container.Resolve<CoroutineHolder>();
        coroutineHolder.StartCoroutine(PerformRoutine(sender, target));

        return true;
    }

    protected virtual AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
        => IsPerforming ? AbilityRequestStatus.Deny : AbilityRequestStatus.Success;

    protected internal IEnumerator PerformRoutine(NetworkBehaviour sender, NetworkBehaviour target)
    {
        IsPerforming = true;

        if (UsagePrepareTime > 0)
            yield return PrepareUsageRoutine();

        OnPerformStarted?.Invoke(Duration);
        yield return OnPerform(sender, target);

        yield return OnPerformed(sender, target);
        OnFinished?.Invoke();

        IsPerforming = false;
    }

    private IEnumerator PrepareUsageRoutine()
    {
        OnPreparationStarted?.Invoke(UsagePrepareTime);
        yield return new WaitForSeconds(UsagePrepareTime);
    }

    protected abstract IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target);

    protected virtual IEnumerator OnPerformed(NetworkBehaviour sender, NetworkBehaviour target)
    {
        SetNextUseTime(CooldownTime);
        yield return null;
    }

    protected void SetNextUseTime(float cooldownTime)
        => _nextUsageTime = Time.time + cooldownTime;

    public override bool Equals(object obj)
    {
        if (obj is not Ability other)
            return false;

        return GetHashCode() == other.GetHashCode()
            && other.CooldownTime == CooldownTime
            && other.UsagePrepareTime == UsagePrepareTime;
    }

    public override int GetHashCode()
        => base.GetHashCode();
}
