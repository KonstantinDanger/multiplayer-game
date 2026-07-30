using Mirror;
using System;
using System.Collections.Generic;

public interface IAbilityUser
{
    event Action<IAbilityPresentationData, float> OnPreparation;
    event Action<IAbilityPresentationData, float> OnPerform;
    event Action<IAbilityPresentationData> OnFinish;

    IReadOnlyList<AbilitySlot> Slots { get; }

    void OnUpdate();
    Ability Use(int index, NetworkBehaviour target = null);
    void Add(Ability ability);
    int FindIndexOf(Ability abilityToUse);
    void Initialize(List<ScriptableAbility> abilities, ParallelAbilityExecutionMatrix executionMatrix);
}
