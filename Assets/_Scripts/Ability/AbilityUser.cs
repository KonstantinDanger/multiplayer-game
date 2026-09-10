using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class AbilityUser : NetworkBehaviour, IAbilityUser
{
    private readonly List<AbilitySlot> _slots = new();
    private readonly List<AbilityInstance> _abilityInstances = new();
    private readonly SyncDictionary<int, AbilitySlotData> _slotsData = new();

    private ParallelAbilityExecutionMatrix _executionMatrix;

    public IReadOnlyList<AbilitySlot> Slots => _slots;
    public IReadOnlyList<AbilityInstance> AbilityInstances => _abilityInstances;

    public event Action<IAbilityPresentationData, float> OnPreparation;
    public event Action<IAbilityPresentationData, float> OnPerform;
    public event Action<IAbilityPresentationData> OnFinish;
    public event Action<IAbilityPresentationData> OnAbilitySet;
    public event Action<AbilitySlotData> OnAbilitySlotStateChange;

    public override void OnStartClient()
        => _slotsData.OnChange += HandleSlotDataChange;

    public override void OnStopClient()
        => _slotsData.OnChange -= HandleSlotDataChange;

    public void Initialize(List<ScriptableAbility> abilities, ParallelAbilityExecutionMatrix executionMatrix)
    {
        if (_slots.Count > 0)
        {
            _abilityInstances.Clear();
            _slots.Clear();
        }

        foreach (ScriptableAbility scriptableAbility in abilities)
        {
            AbilityInstance instance = new(scriptableAbility.GetNew(), scriptableAbility);
            Add(instance);
            OnAbilitySet?.Invoke(scriptableAbility);
        }

        _executionMatrix = executionMatrix;
    }

    public void OnUpdate()
    {
        foreach (var ability in _slots)
            ability?.Update();
    }

    public void Add(AbilityInstance instance)
    {
        Ability ability = instance.ability;

        _abilityInstances.Add(instance);

        _slotsData[_slots.Count] = new();

        _slots.Add(new AbilitySlot(instance, this,
                (a, duration) => HandleAbilityPreparation(a, duration),
                (a, duration) => HandleAbilityPerform(a, duration),
                (a) => HandleAbilityFinish(a),
                (data) => UpdateSlotData(_slotsData.Count - 1, data)));

        if (ability is ICacheAbilities abilityCacher)
        {
            var cached = abilityCacher.CacheAbilities().ToList();

            cached.ForEach(cachedInstance =>
            {
                _abilityInstances.Add(cachedInstance);
            });
        }
    }

    /// <summary>
    /// Returns -1 if target ability is not found
    /// </summary>
    /// <param name="abilityToUse"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int FindIndexOf(Ability abilityToUse)
        => _slots.FindIndex(slot => slot.AbilityInstance.ability.Equals(abilityToUse));

    public virtual Ability Use(int index, NetworkBehaviour target = null)
    {
        if (index > _slots.Count)
            throw new Exception("Wrong index");

        AbilitySlot selectedSlot = _slots[index];

        if (selectedSlot == null)
            return null;

        if (!RequestUsage(selectedSlot))
            return null;

        selectedSlot.Use(this, target);

        //if (selectedSlot.Use(this, target))
        //    OnPreparation.Invoke(SetupData(selectedSlot.Ability));

        return selectedSlot.AbilityInstance.ability;
    }

    private void HandleSlotDataChange(SyncDictionary<int, AbilitySlotData>.Operation operation, int slotIndex, AbilitySlotData data)
        => OnAbilitySlotStateChange?.Invoke(data);

    private void UpdateSlotData(int slotIndex, AbilitySlotData data)
    {
        _slotsData[slotIndex] = new()
        {
            AccumulatedCharges = data.AccumulatedCharges,
            MaxCharges = data.MaxCharges,
        };
    }

    private bool RequestUsage(AbilitySlot slot)
    {
        if (_executionMatrix == null)
            return true;

        if (IsAnyAbilityPerforming(out Ability performingOne))
        {
            if (!_executionMatrix.CanParallelExecute(performingOne, slot.AbilityInstance.ability))
                return false;
        }

        return true;
    }

    private bool IsAnyAbilityPerforming(out Ability performingOne)
    {
        performingOne = null;

        foreach (var slot in _slots)
        {
            if (slot.AbilityInstance.ability.IsPerforming)
            {
                performingOne = slot.AbilityInstance.ability;
                return true;
            }
        }

        return false;
    }

    private void HandleAbilityPreparation(Ability ability, float duration)
        => OnPreparation?.Invoke(FindPresentationData(ability), duration);

    private void HandleAbilityPerform(Ability ability, float duration)
        => OnPerform?.Invoke(FindPresentationData(ability), duration);

    private void HandleAbilityFinish(Ability ability)
        => OnFinish?.Invoke(FindPresentationData(ability));

    private IAbilityPresentationData FindPresentationData(Ability ability)
        => _abilityInstances
        .Find(instance => instance.ability
        .Equals(ability))?.presentationData;

    //private UseAbilityData SetupData(Ability ability)
    //{
    //    UseAbilityData data = new();

    //    if (ability.PreparationAnimation == null)
    //    {
    //        data.UsagePreparationTime = 0f;
    //        data.UsagePreparationAnimDuration = 0f;
    //        data.PreparationAnimationName = string.Empty;
    //    }
    //    else if (ability.UsageAnimation == null)
    //    {
    //        data.UsageAnimationName = string.Empty;
    //    }
    //    else
    //    {
    //        data = new UseAbilityData()
    //        {
    //            UsagePreparationAnimDuration = ability.PreparationAnimation.averageDuration,
    //            PreparationAnimationName = ability.PreparationAnimation.name,
    //            UsagePreparationTime = ability.UsagePrepareTime,

    //            UsageAnimationName = ability.UsageAnimation.name,
    //        };
    //    }

    //    return data;
    //}
}
