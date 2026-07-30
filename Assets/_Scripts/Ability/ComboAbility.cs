using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboAbility : Ability
{
    [SerializeField] private List<ScriptableAbility> _scriptableAbilities = new();
    //[SerializeField, Range(0f, 1f)] private float _nextAttackCacheProgressThreshold = 0.5f;
    [SerializeField, Range(0f, 2f)] private float _abilityUsageRecoveryTime = 0.5f;

    private readonly List<Ability> _abilities = new();

    private int _currentAttackIndex = 0;
    private bool _cacheNext;

    private Ability CurrentAbility => _abilities[_currentAttackIndex];
    private bool IsComboEnded => _currentAttackIndex >= _abilities.Count - 1;

    public override float Duration => CurrentAbility.Duration;

    protected override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        _cacheNext = true;
        return AbilityRequestStatus.InterruptWithSuccess;
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            yield break;

        if (_abilities.Count == 0)
            CacheAbilities();

        while (true)
        {
            _cacheNext = false;

            UnityEngine.Debug.Log("Current attack idx " + _currentAttackIndex);
            yield return CurrentAbility.PerformRoutine(sender, target);

            if (CurrentAbility is WeaponSpawnAbility weaponAbility && weaponAbility.Skipped)
            {
                IncrementCombo();
                continue;
            }

            if (IsComboEnded)
            {
                break;
            }

            yield return Recovery();

            if (!_cacheNext)
            {
                break;
            }

            IncrementCombo();
        }

        yield return null;
    }

    protected override IEnumerator OnPerformed(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (IsComboEnded)
        {
            UnityEngine.Debug.Log("setting combo end cooldown ");
            SetNextUseTime(CooldownTime);
        }

        ResetCombo();
        yield return null;
    }

    private IEnumerator Recovery()
    {
        yield return new WaitForSeconds(_abilityUsageRecoveryTime);
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

    private void ResetCombo()
        => _currentAttackIndex = 0;

    private void CacheAbilities()
        => _scriptableAbilities
            .ForEach(sa => _abilities
            .Add(sa
            .GetNew()));
}
