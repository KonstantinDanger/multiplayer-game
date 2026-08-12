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

    public override float Duration => _spawnTime;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        Skipped = false;

        if (!sender.TryGetComponent(out WeaponUserComponent weaponUserComponent))
            yield break;

        WeaponUser weaponUser = weaponUserComponent.WeaponUser;

        if (weaponUser == null)
        {
            UnityEngine.Debug.Log("No weapon user found while performing weapon spawn ability ");
            yield break;
        }

        if (WeaponExists(weaponUser))
        {
            Skipped = true;
            yield break;
        }

        yield return new WaitForSeconds(_spawnTime);

        SpawnWeapon(weaponUser);
    }

    private bool WeaponExists(WeaponUser user)
        => user.HasEquippedAny();

    private void SpawnWeapon(WeaponUser user)
        => user.Equip(_weapon);
}
