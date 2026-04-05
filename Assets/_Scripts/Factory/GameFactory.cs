using Mirror;
using UnityEngine;

public class GameFactory : NetworkBehaviour
{
    public CustomNetworkManager CreateNetworkManager(CustomNetworkManager networkManagerPrefab, Transform transform)
        => Instantiate(networkManagerPrefab, transform);

    [Server]
    public Zone SpawnZone(Zone zonePrefab, Vector3 position)
    {
        var zone = Instantiate(zonePrefab, position, Quaternion.identity);
        NetworkServer.Spawn(zone.gameObject);
        return zone;
    }

    public DamageText SpawnDamageText(DamageText damageTextPrefab, Transform transform)
    {
        var text = Instantiate(damageTextPrefab, transform);

        return text;
    }

    public Projectile SpawnProjectile(Projectile projectile, Vector3 direction, float flightSpeed, NetworkBehaviour sender, Damage damage, Transform transform, Quaternion rotation, float scaleMultiplier = 1, Transform parent = null)
    {
        Projectile proj = Instantiate(projectile, transform.position, rotation, parent);
        proj.transform.localScale *= scaleMultiplier;

        SpawnOnServer(proj.gameObject, sender.gameObject);

        ProjectileData data = new(proj.netId)
        {
            Direction = direction,
            SenderId = sender.netId,
            Speed = flightSpeed,
            Damage = damage
        };

        int excludeMask = ~0;

        excludeMask &= ~(1 << 0);

        excludeMask &= ~damage.AttackLayers.value;

        proj.Collider.excludeLayers = excludeMask;

        proj.Initialize(data);

        return proj;
    }

    public Entity SpawnEntity(Entity entityPrefab, Vector3 summonPosition, Quaternion rotation, NetworkBehaviour owner)
    {
        var summon = Instantiate(entityPrefab, summonPosition, rotation, null);

        SpawnOnServer(summon.gameObject, owner.gameObject);

        return summon;
    }

    private void SpawnOnServer(GameObject gameObject, GameObject owner)
    {
        if (owner.TryGetComponent(out Player _))
            NetworkServer.Spawn(gameObject, owner);
        else
            NetworkServer.Spawn(gameObject);
    }
}
