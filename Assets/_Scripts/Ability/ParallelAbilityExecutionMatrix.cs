using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parallel Ability Execution Matrix", fileName = "ParallelAbilityExecutionMatrix_")]
public class ParallelAbilityExecutionMatrix : ScriptableObject
{
    [SerializeField] private List<ScriptableAbility> _abilities = new();
    [SerializeField] private TransitionMatrix _transitions = new();

    public bool CanParallelExecute(Ability current, Ability next)
    {
        if (current == null || next == null)
            return false;

        int currentIndex = GetIndexOf(current);
        int nextIndex = GetIndexOf(next);

        if (currentIndex < 0 || nextIndex < 0)
        {
            Debug.LogWarning(
                $"Ability '{current?.Name}' or '{next?.Name}' is not available.");

            return false;
        }

        return _transitions.Get(currentIndex, nextIndex, _abilities.Count);
    }

    private int GetIndexOf(Ability ability)
    {
        var scriptableAbility = _abilities.Find(a => a.GetAbilityType() == ability.GetType());
        return _abilities.IndexOf(scriptableAbility);
    }
}
