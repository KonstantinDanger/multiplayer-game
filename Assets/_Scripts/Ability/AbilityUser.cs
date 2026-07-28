using Mirror;
using System;
using System.Collections.Generic;

public class AbilityUser : NetworkBehaviour, IAbilityUser
{
    private readonly List<AbilitySlot> _slots = new();
    private ParallelAbilityExecutionMatrix _executionMatrix;

    public IReadOnlyList<AbilitySlot> Slots => _slots;

    public event Action<UseAbilityData> OnAbilityStartUsing;

    public void Initialize(List<Ability> abilities, ParallelAbilityExecutionMatrix executionMatrix)
    {
        if (_slots.Count > 0)
            _slots.Clear();

        abilities.ForEach(ability => Add(ability));
        _executionMatrix = executionMatrix;
    }

    public void OnUpdate()
    {
        foreach (var ability in _slots)
            ability?.Update();
    }

    public void Add(Ability ability)
        => _slots.Add(new(ability));

    /// <summary>
    /// Returns -1 if target ability is not found
    /// </summary>
    /// <param name="abilityToUse"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int FindIndexOf(Ability abilityToUse)
        => _slots.FindIndex(handler => handler.Ability.Equals(abilityToUse));

    //public virtual Ability Use(Ability abilityToUse, NetworkBehaviour target = null)
    //{
    //    AbilityHandler ability = _abilities.Find(handler => handler.Ability.Equals(abilityToUse));

    //    if (ability.Perform(this, target))
    //        OnAbilityStartUsing.Invoke(SetupData(ability));

    //    return ability;
    //}

    public virtual Ability Use(int index, NetworkBehaviour target = null)
    {
        if (index > _slots.Count)
            throw new System.Exception("Wrong index");

        AbilitySlot selectedSlot = _slots[index];

        if (selectedSlot == null)
            return null;

        if (!RequestUsage(selectedSlot))
            return null;

        if (selectedSlot.Use(this, target))
            OnAbilityStartUsing.Invoke(SetupData(selectedSlot.Ability));

        return selectedSlot.Ability;
    }

    private bool RequestUsage(AbilitySlot slot)
    {
        if (_executionMatrix == null)
            return true;

        if (IsAnyAbilityPerforming(out Ability performingOne))
        {
            if (!_executionMatrix.CanParallelExecute(performingOne, slot.Ability))
                return false;
        }

        return true;
    }

    private bool IsAnyAbilityPerforming(out Ability performingOne)
    {
        performingOne = null;

        foreach (var slot in _slots)
        {
            if (slot.Ability.IsPerforming)
            {
                performingOne = slot.Ability;
                return true;
            }
        }

        return false;
    }

    private UseAbilityData SetupData(Ability ability)
    {
        UseAbilityData data = new();

        if (ability.PreparationAnimation == null)
        {
            data.UsagePreparationTime = 0f;
            data.UsagePreparationAnimDuration = 0f;
            data.PreparationAnimationName = string.Empty;
        }
        else if (ability.UsageAnimation == null)
        {
            data.UsageAnimationName = string.Empty;
        }
        else
        {
            data = new UseAbilityData()
            {
                UsagePreparationAnimDuration = ability.PreparationAnimation.averageDuration,
                PreparationAnimationName = ability.PreparationAnimation.name,
                UsagePreparationTime = ability.UsagePrepareTime,

                UsageAnimationName = ability.UsageAnimation.name,
            };
        }

        return data;
    }
}
