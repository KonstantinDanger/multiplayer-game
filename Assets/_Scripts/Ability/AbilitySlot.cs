using Mirror;
using System;
using UnityEngine;

[Serializable]
public class AbilitySlot : IGauge
{
    public event Action OnUsageDeny;
    public event Action OnValueChanged;

    public int AccumulationCharges { get; private set; }

    public IGauge AccumulationGauge { get; private set; } = null;

    private Ability Ability => AbilityInstance.ability;

    public AbilityInstance AbilityInstance { get; private set; }

    public float CurrentGaugeValue => Ability.RechargeProgress;
    public float MaxGaugeValue => Ability.CooldownTime;

    public AbilitySlot(AbilityInstance instance,
    Action<Ability, float> handleAbilityPreparation,
    Action<Ability, float> handleAbilityPerform,
    Action<Ability> handleAbilityFinish)
    {
        AbilityInstance = instance;

        Ability.OnPreparationStarted += handleAbilityPreparation;
        Ability.OnPerformStarted += handleAbilityPerform;
        Ability.OnFinished += handleAbilityFinish;

        if (Ability is CumulativeAbility cumulative)
            AccumulationGauge = new CumulativeAbilityGauge(cumulative);
    }

    public void Update()
    {
        AccumulationCharges = AccumulationGauge == null ? -1 : Mathf.FloorToInt(AccumulationGauge.CurrentGaugeValue);

        OnValueChanged?.Invoke();
    }

    public bool Use(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!Ability.Perform(sender, target))
        {
            OnUsageDeny?.Invoke();
            return false;
        }

        return true;
    }
}
