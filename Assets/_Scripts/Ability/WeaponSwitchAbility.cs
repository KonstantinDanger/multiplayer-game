using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class WeaponSwitchAbility : Ability
{
    [Header("If set to false ability will try to switch to the previous weapon")]
    [SerializeField] private bool _switchToNext;
    [SerializeField, Range(0f, 10f)] private float _switchTime;

    public override float Duration => base.Duration + _switchTime;

    private MultipleWeaponsUser _weaponUser;

    protected internal override AbilityRequestStatus OnPerformRequested(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!sender.TryGetComponent(out WeaponUserComponent userComponent))
            return AbilityRequestStatus.Deny;

        if (userComponent.WeaponUser == null)
        {
            UnityEngine.Debug.Log("No weapon user found while performing weapon switch ability ");
            return AbilityRequestStatus.Deny;
        }

        if (userComponent.WeaponUser is not MultipleWeaponsUser weaponUser)
            return AbilityRequestStatus.Deny;

        _weaponUser = weaponUser;

        if (!_weaponUser.HasEquippedAny() || _weaponUser.WeaponsAmount == 1)
            return AbilityRequestStatus.Deny;

        return base.OnPerformRequested(sender, target);
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        yield return new WaitForSeconds(_switchTime);

        if (_switchToNext)
            _weaponUser.EquipNext();
        else
            _weaponUser.EquipPrev();

        yield return null;
    }
}
