using Mirror;
using System;
using UnityEngine;

[Serializable]
public class AbilityHandler : Ability, IGauge
{
    public event Action OnValueChanged;
    public Ability Ability { get; private set; }

    public override float CooldownTime => Ability.CooldownTime;
    public override string Name => Ability.Name;

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

    public AbilityHandler(Ability ability)
        => Ability = ability;

    protected override void OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!IsReadyToUse())
            return;

        if (!Ability.Perform(sender, target))
            return;

        SetNextUseTime();
    }

    public void Update()
    {
        if (!IsReadyToUse())
            OnValueChanged?.Invoke();
    }

    private bool IsReadyToUse()
        => Time.time >= _nextUsageTime;

    private void SetNextUseTime()
        => _nextUsageTime = Time.time + CooldownTime;
}
