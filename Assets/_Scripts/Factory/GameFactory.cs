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

    public Projectile SpawnProjectile(Projectile projectile, Vector3 direction, float flightSpeed, NetworkBehaviour sender, Damage damage, Transform transform, Quaternion rotation, Transform parent = null)
    {
        Projectile proj = Instantiate(projectile, transform.position, rotation, parent);
        NetworkServer.Spawn(proj.gameObject, sender.gameObject);

        ProjectileData data = new(proj.netId)
        {
            Direction = direction,
            SenderId = sender.netId,
            Speed = flightSpeed,
            Damage = damage
        };

        proj.Initialize(data);

        return proj;
    }
}
