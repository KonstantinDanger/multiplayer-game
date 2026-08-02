using Mirror;
using System;

[Serializable]
public class AbilitySlot : IGauge
{
    public event Action OnUsageDeny;
    public event Action OnValueChanged;

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
    }

    public void Update()
        => OnValueChanged?.Invoke();

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
