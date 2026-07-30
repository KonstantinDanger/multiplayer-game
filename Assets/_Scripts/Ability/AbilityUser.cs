using Mirror;
using System;
using System.Collections.Generic;

public class AbilityUser : NetworkBehaviour, IAbilityUser
{
    private readonly List<AbilitySlot> _slots = new();
    private readonly Dictionary<Ability, ScriptableAbility> _scriptableAbilities = new();

    private ParallelAbilityExecutionMatrix _executionMatrix;

    public IReadOnlyList<AbilitySlot> Slots => _slots;
    public IReadOnlyDictionary<Ability, ScriptableAbility> ScriptableAbilities => _scriptableAbilities;

    public event Action<IAbilityPresentationData, float> OnPreparation;
    public event Action<IAbilityPresentationData, float> OnPerform;
    public event Action<IAbilityPresentationData> OnFinish;
    public event Action<IAbilityPresentationData> OnAbilitySet;

    public void Initialize(List<ScriptableAbility> abilities, ParallelAbilityExecutionMatrix executionMatrix)
    {
        if (_slots.Count > 0)
        {
            _scriptableAbilities.Clear();
            _slots.Clear();
        }

        foreach (ScriptableAbility scriptableAbility in abilities)
        {
            Ability instance = scriptableAbility.GetNew();
            _scriptableAbilities.Add(instance, scriptableAbility);
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

    public void Add(Ability ability)
    {
        _slots.Add(new AbilitySlot(ability,
                (duration) => HandleAbilityPreparation(ability, duration),
                (duration) => HandleAbilityPerform(ability, duration),
                () => HandleAbilityFinish(ability)));
    }

    /// <summary>
    /// Returns -1 if target ability is not found
    /// </summary>
    /// <param name="abilityToUse"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int FindIndexOf(Ability abilityToUse)
        => _slots.FindIndex(handler => handler.Ability.Equals(abilityToUse));

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

    private void HandleAbilityPreparation(Ability ability, float duration)
        => OnPreparation?.Invoke(FindPresentationData(ability), duration);

    private void HandleAbilityPerform(Ability ability, float duration)
        => OnPerform?.Invoke(FindPresentationData(ability), duration);

    private void HandleAbilityFinish(Ability ability)
        => OnFinish?.Invoke(FindPresentationData(ability));

    private IAbilityPresentationData FindPresentationData(Ability ability)
    {
        if (!_scriptableAbilities.TryGetValue(key: ability, out ScriptableAbility scriptable))
            throw new Exception("No abilities found");

        return scriptable;
    }

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
