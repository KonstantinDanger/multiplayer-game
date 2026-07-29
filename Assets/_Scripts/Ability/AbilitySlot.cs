using Mirror;
using System;

[Serializable]
public class AbilitySlot : IGauge
{
    public event Action<float> OnPreparation;
    public event Action<float> OnPerformed;
    public event Action OnFinish;

    public event Action OnUsageDeny;
    public event Action OnValueChanged;

    public Ability Ability { get; private set; }

    public float CurrentGaugeValue => Ability.RechargeProgress;
    public float MaxGaugeValue => Ability.CooldownTime;

    public AbilitySlot(Ability ability)
    {
        Ability = ability;

        Ability.Started += duration => OnPreparation.Invoke(duration);
        Ability.Performed += duration => OnPerformed.Invoke(duration);
        Ability.Finished += () => OnFinish.Invoke();
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
