using Mirror;
using System;
using System.Collections.Generic;

public interface IAbilityUser
{
    event Action<IAbilityPresentationData, float> OnPreparation;
    event Action<IAbilityPresentationData, float> OnPerform;
    event Action<IAbilityPresentationData> OnFinish;

    IReadOnlyList<AbilitySlot> Slots { get; }
    IReadOnlyList<AbilityInstance> AbilityInstances { get; }

    void OnUpdate();
    Ability Use(int index, NetworkBehaviour target = null);
    void Add(AbilityInstance instance);
    int FindIndexOf(Ability abilityToUse);
    void Initialize(List<ScriptableAbility> abilities, ParallelAbilityExecutionMatrix executionMatrix);
    IAbilityPresentationData GetPresentationData(AbilitySlot abilitySlot);
}
