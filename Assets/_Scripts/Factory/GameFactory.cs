using Mirror;
using System.Collections;
using UnityEngine;

public class GameFactory : NetworkBehaviour
{
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

    public T Create<T>(T uiPrefab, Transform transform) where T : UI
        => Instantiate(uiPrefab, transform);

    [Server]
    public Projectile SpawnProjectile(Projectile projectile, Vector3 direction, float flightSpeed, NetworkBehaviour sender, Damage damage, Transform transform, Quaternion rotation, float scaleMultiplier = 1, Transform parent = null)
    {
        Projectile proj = Instantiate(projectile, transform.position, rotation, parent);
        proj.transform.localScale *= scaleMultiplier;

        ProjectileData data = new(proj.netId)
        {
            Direction = direction,
            SenderId = sender.netId,
            Speed = flightSpeed,
            Damage = damage
        };

        //int excludeMask = ~0;

        //excludeMask &= ~(1 << 0);

        //excludeMask &= ~damage.AttackLayers.value;

        //proj.Collider.excludeLayers = excludeMask;

        proj.Initialize(data);

        SpawnOnServer(proj.gameObject, sender.gameObject);

        return proj;
    }

    [Server]
    public Entity SummonEntity(Entity entityPrefab, Vector3 summonPosition, Quaternion rotation, NetworkBehaviour owner)
    {
        var summon = Instantiate(entityPrefab, summonPosition, rotation, null);

        SpawnOnServer(summon.gameObject, owner.gameObject);

        return summon;
    }

    [Server]
    public void SpawnDecoy(NetworkBehaviour sender, float lifetime, Vector3 position, Quaternion rotation)
    {
        string decoyName = $"{sender.name}'s decoy";

        var decoy = Instantiate(Utils.GetEmptyNetGameObjectPrefab());
        decoy.name = decoyName;

        var visuals = Instantiate(Utils.GetEmptyNetGameObjectPrefab());
        visuals.name = $"{decoyName} visuals";
        visuals.transform.SetParent(decoy.transform);

        SkinnedMeshRenderer[] sMeshRenderers = sender.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (sMeshRenderers.Length > 0)
        {
            foreach (SkinnedMeshRenderer mRend in sMeshRenderers)
            {
                Utils.CloneMeshRenderer(mRend, visuals.transform);
            }
        }

        visuals.transform.Rotate(gameObject.transform.right, -90f);

        decoy.transform.SetPositionAndRotation(position, rotation);
        SpawnOnServer(decoy.gameObject, sender.gameObject);
        ServerDestroy(decoy.gameObject, lifetime);
    }

    [Server]
    private void ServerDestroy(GameObject gameObject, float lifeTime)
        => StartCoroutine(DestroyGameObjectRoutine(gameObject, lifeTime));

    private IEnumerator DestroyGameObjectRoutine(GameObject gameObject, float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        NetworkServer.Destroy(gameObject);
    }

    [Server]
    private void SpawnOnServer(GameObject gameObject, GameObject owner)
    {
        if (owner.TryGetComponent(out Player player))
            NetworkServer.Spawn(gameObject, player.connectionToClient);
        else
            NetworkServer.Spawn(gameObject);
    }
}
