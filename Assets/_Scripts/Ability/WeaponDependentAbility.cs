using AYellowpaper.SerializedCollections;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponDependentAbility : Ability, IPresentInnerAbility, ICacheAbilities
{
    [SerializeField]
    private SerializedDictionary<ScriptableWeapon, ScriptableAbility> _weaponAbilities = new();

    private readonly Dictionary<ScriptableWeapon, Ability> _weaponAbilityInstances = new();

    private Ability _selectedAbilityInstance;
    private WeaponUser _user;

    public IAbilityPresentationData GetInnerAbilityPresentation(NetworkBehaviour owner)
    {
        if (_user == null)
            if (owner.TryGetComponent(out WeaponUserComponent weaponComponent))
                _user = weaponComponent.WeaponUser;

        return _weaponAbilities[_user?.SelectedWeapon];

    }

    protected internal override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_weaponAbilities.Count == 0)
            return AbilityRequestStatus.Deny;

        if (!sender.TryGetComponent(out WeaponUserComponent weaponComponent))
        {
            UnityEngine.Debug.LogError("Weapon user component is not found");

            return AbilityRequestStatus.Deny;
        }

        _user = weaponComponent.WeaponUser;

        if (_user == null)
        {
            UnityEngine.Debug.LogError("No weapon user found while using weapon dependent ability");

            return AbilityRequestStatus.Deny;
        }

        _selectedAbilityInstance = _weaponAbilityInstances[_user.SelectedWeapon];

        if (_selectedAbilityInstance.IsRecharging)
            return AbilityRequestStatus.Deny;

        return _selectedAbilityInstance.OnPerformRequested(sender, target);
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        ScriptableWeapon selectedWeapon = _user.SelectedWeapon;

        if (!_weaponAbilities.ContainsKey(selectedWeapon))
        {
            UnityEngine.Debug.LogError("No such weapon found in list: " + selectedWeapon.Name);
            yield break;
        }

        yield return _selectedAbilityInstance.PerformRoutine(sender, target);
    }

    public IEnumerable<AbilityInstance> CacheAbilities()
    {
        List<AbilityInstance> instances = new();

        foreach (var weapon in _weaponAbilities)
        {
            ScriptableAbility scriptableAbility = weapon.Value;
            Ability subAbilityInstance = scriptableAbility.GetNew();

            subAbilityInstance.OnPreparationStarted += (current, duration) => RaisePreparationStarted(current, duration);
            subAbilityInstance.OnPerformStarted += (current, duration) => RaisePerformStarted(current, duration);
            subAbilityInstance.OnFinished += current => RaiseFinished(current);

            if (subAbilityInstance is ICacheAbilities cacher)
            {
                IEnumerable<AbilityInstance> cachedInner = cacher.CacheAbilities();
                instances.AddRange(cachedInner);
            }

            _weaponAbilityInstances.Add(weapon.Key, subAbilityInstance);

            instances.Add(new AbilityInstance(subAbilityInstance, scriptableAbility));
        }

        return instances;
    }

    protected override void OnPerformStartedEventInvoke() { return; }
}
