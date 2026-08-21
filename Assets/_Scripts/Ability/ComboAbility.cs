using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ComboAbility : Ability, ICacheAbilities
{
    [SerializeField] private List<ScriptableAbility> _scriptableAbilities = new();
    [SerializeField, Range(0f, 2f)] private float _abilityUsageRecoveryTime = 0.5f;

    private readonly List<Ability> _abilities = new();

    private int _currentAttackIndex = 0;
    private bool _cacheNext;

    private Ability CurrentAbility => _abilities[_currentAttackIndex];
    private bool IsComboEnded => _currentAttackIndex >= _abilities.Count - 1;

    public override float Duration { get; protected set; } = 0f;

    public IEnumerable<AbilityInstance> CacheAbilities()
    {
        List<AbilityInstance> instances = new();

        _scriptableAbilities.ForEach(scriptableAbility =>
        {
            var subAbilityInstance = scriptableAbility.GetNew();

            subAbilityInstance.OnPreparationStarted += (current, duration) => TryInvoke(() => RaisePreparationStarted(current, duration));
            subAbilityInstance.OnPerformStarted += (current, duration) => TryInvoke(() => RaisePerformStarted(current, duration));
            subAbilityInstance.OnFinished += current => TryInvoke(() => RaiseFinished(current));

            _abilities.Add(subAbilityInstance);

            instances.Add(new AbilityInstance(subAbilityInstance, scriptableAbility));
        });

        return instances;
    }

    private void TryInvoke(Action action)
    {
        if (CurrentAbility is WeaponSpawnAbility weaponSpawnAbility && weaponSpawnAbility.Skipped)
            return;

        action?.Invoke();
    }

    protected override void OnPerformStartedEventInvoke() { return; }

    protected internal override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!IsPerforming)
            return AbilityRequestStatus.Success;

        _cacheNext = true;

        return AbilityRequestStatus.InterruptWithSuccess;
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            yield break;

        while (true)
        {
            _cacheNext = false;
            Duration = CurrentAbility.Duration;
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
            SetNextUseTime(CooldownTime);

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
}