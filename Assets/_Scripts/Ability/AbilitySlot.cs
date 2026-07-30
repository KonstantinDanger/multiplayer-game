using Mirror;
using System;

[Serializable]
public class AbilitySlot : IGauge
{
    public event Action OnUsageDeny;
    public event Action OnValueChanged;

    public Ability Ability { get; private set; }

    public float CurrentGaugeValue => Ability.RechargeProgress;
    public float MaxGaugeValue => Ability.CooldownTime;

    public AbilitySlot(Ability ability,
    Action<float> handleAbilityPreparation,
    Action<float> handleAbilityPerform,
    Action handleAbilityFinish)
    {
        Ability = ability;

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
