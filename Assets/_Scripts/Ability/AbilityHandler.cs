using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class AbilityHandler : Ability, IGauge
{
    public event Action OnValueChanged;
    public Ability Ability { get; private set; }

    public float CurrentGaugeValue
    {
        get
        {
            float elapsedTime = Time.time - (_nextUsageTime - CooldownTime);

            return Mathf.Clamp(elapsedTime, 0f, CooldownTime);
        }
    }
    public float MaxGaugeValue => CooldownTime;

    private float _nextUsageTime = 0;

    private readonly CoroutineHolder _coroutineHolder;

    public AbilityHandler(Ability ability)
    {
        Ability = ability;

        Name = ability.Name;
        CooldownTime = ability.CooldownTime;
        PreparationAnimation = ability.PreparationAnimation;
        UsageAnimation = ability.UsageAnimation;
        UsagePrepareTime = ability.UsagePrepareTime;

        _coroutineHolder = ServiceLocator.Container.Resolve<CoroutineHolder>();
    }

    protected override bool OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!IsReadyToUse())
            return false;

        _coroutineHolder.StartCoroutine(OnPerformRoutine(sender, target));

        return true;
    }

    public void Update()
    {
        if (!IsReadyToUse())
            OnValueChanged?.Invoke();
    }

    public bool IsReadyToUse()
        => Time.time >= _nextUsageTime && !IsPerforming;

    private void SetNextUseTime()
        => _nextUsageTime = Time.time + CooldownTime;

    private IEnumerator OnPerformRoutine(NetworkBehaviour sender, NetworkBehaviour target)
    {
        IsPerforming = true;

        if (UsagePrepareTime > 0)
            yield return PrepareUsageRoutine();

        if (!Ability.Perform(sender, target))
            yield break;

        SetNextUseTime();

        IsPerforming = false;
    }

    private IEnumerator PrepareUsageRoutine()
    {
        yield return new WaitForSeconds(UsagePrepareTime);
    }
}
