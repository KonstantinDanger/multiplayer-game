using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class WeaponSpawnAbility : Ability
{
    [SerializeField] private ScriptableWeapon _weapon;
    [SerializeField, Range(0f, 10f)] private float _spawnTime = 2f;

    public bool Skipped { get; private set; }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        Skipped = false;

        if (!sender.TryGetComponent(out WeaponUser weaponUser))
            yield break;

        if (WeaponExists(weaponUser))
        {
            Skipped = true;
            yield break;
        }

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
