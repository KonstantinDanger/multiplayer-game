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
}
