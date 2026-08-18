using System;
using UnityEngine;

[Serializable]
public class AddWeaponUpgrade : Upgrade
{
    [SerializeField] private ScriptableWeapon _weapon;

    protected override void OnObtain(GameObject target)
    {
        if (!target.TryGetComponent(out WeaponUserComponent weaponUser))
            return;

        if (weaponUser.WeaponUser is not MultipleWeaponsUser weaponsUser)
            return;

        if (weaponsUser.HasEquipped(_weapon))
            return;

        weaponsUser.Equip(_weapon);
    }
}
