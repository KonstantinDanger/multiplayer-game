using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComboAbility : Ability
{
    [SerializeField] private List<ScriptableAbility> _scriptableAbilities = new();
    [SerializeField, Range(0f, 1f)] private float _nextAttackCacheProgressThreshold = 0.5f;

    private readonly List<Ability> _abilities = new();

    private int _currentAttackIndex = 0;

    private bool _cachedNext;

    private Ability CurrentAbility => _abilities[_currentAttackIndex];
    private bool IsComboEnded => _currentAttackIndex >= _abilities.Count - 1;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            yield break;

        if (_abilities.Count == 0)
            CacheAbilities();

        //if (CurrentAttack.IsPerforming && !_cachedNext /*&& CurrentAttack.PerformTime >= _nextAttackCacheProgressThreshold*/)
        //{
        //    _cachedNext = true;
        //}

        UnityEngine.Debug.Log("Current attack idx " + _currentAttackIndex);

        CurrentAbility.Perform(sender, target);

        while (CurrentAbility.IsPerforming)
            yield return null;

        yield break;
    }

    private void IncrementCombo()
    {
        if (IsComboEnded)
        {
            _currentAttackIndex = 0;
            return;
        }

        _currentAttackIndex++;
    }

    protected override IEnumerator OnPerformed(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (IsComboEnded)
        {
            SetNextUseTime(CooldownTime);
        }
        //else if (_cachedNext)
        //{
        //    Perform(sender, target);
        //    _cachedNext = false;
        //}

        IncrementCombo();
        yield return null;
    }

    private void CacheAbilities()
        => _scriptableAbilities
            .ForEach(sa => _abilities
            .Add(sa
            .GetNew()));
}
