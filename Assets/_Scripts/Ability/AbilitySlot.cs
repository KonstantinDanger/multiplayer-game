using Mirror;
using System;

[Serializable]
public class AbilitySlot
{
    public event Action OnUsageDeny;

    private AbilitySlotData _data;
    private readonly Action<AbilitySlotData> OnSlotStateUpdate;
    private readonly CumulativeAbility _cumulativeAbility = null;

    private Ability Ability => AbilityInstance.ability;
    public AbilityInstance AbilityInstance { get; private set; }

    public AbilitySlot(AbilityInstance instance,
    Action<Ability, float> handleAbilityPreparation,
    Action<Ability, float> handleAbilityPerform,
    Action<Ability> handleAbilityFinish,
    Action<AbilitySlotData> onSlotStateUpdate)
    {
        AbilityInstance = instance;

        Ability.OnPreparationStarted += handleAbilityPreparation;
        Ability.OnPerformStarted += handleAbilityPerform;
        Ability.OnFinished += handleAbilityFinish;

        if (Ability is CumulativeAbility cumulative)
        {
            _data.MaxCharges = cumulative.MaxCharges;
            _cumulativeAbility = cumulative;
        }

        OnSlotStateUpdate = onSlotStateUpdate;
    }

    public void Update()
    {
        _data.AccumulatedCharges = _cumulativeAbility == null ? -1 : _cumulativeAbility.AccumulatedCharges;
        _data.RechargeProgress = Ability.RechargeProgress / Ability.CooldownTime;

        OnSlotStateUpdate?.Invoke(_data);
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
