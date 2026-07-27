using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class WeaponSpawnAbility : Ability
{
    [SerializeField] private ScriptableWeapon _weapon;
    [SerializeField, Range(0f, 10f)] private float _spawnTime = 2f;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (!sender.TryGetComponent(out WeaponUser weaponUser))
            yield break;

        //if weapon exists -> execute next ability in combo if it is in the combo
        if (WeaponExists(weaponUser))
            yield break;

        yield return new WaitForSeconds(_spawnTime);

        SpawnWeapon(weaponUser);
    }

    private bool WeaponExists(WeaponUser user)
        => user.HasEquipped();

    private void SpawnWeapon(WeaponUser user)
    {
        if (WeaponExists(user))
            return;

        user.Equip(_weapon);
    }
}
