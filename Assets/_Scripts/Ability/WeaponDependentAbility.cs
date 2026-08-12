using AYellowpaper.SerializedCollections;
using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class WeaponDependentAbility : Ability
{
    [SerializeField]
    private SerializedDictionary<ScriptableWeapon, ScriptableAbility> _weaponAbilities = new();

    private WeaponUser _user;

    protected override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
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

        return base.OnPerformRequested(sender, target);
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        ScriptableWeapon selectedWeapon = _user.SelectedWeapon;

        if (!_weaponAbilities.ContainsKey(selectedWeapon))
        {
            UnityEngine.Debug.LogError("No such weapon found in list: " + selectedWeapon.Name);
            yield break;
        }

        ScriptableAbility selectedAbility = _weaponAbilities[selectedWeapon];

        // Add ability caching as in a combo ability
        Ability instance = selectedAbility.GetNew();

        yield return instance.PerformRoutine(sender, target);
    }
}
