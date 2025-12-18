using Mirror;
using UnityEngine;

public class ProjectileFactory
{
    public Projectile Create(Projectile prefab, Transform shootPosition, Vector3 shootDirection, NetworkBehaviour sender)
    {
        Projectile projectile = Object.Instantiate(prefab, shootPosition.position, Quaternion.LookRotation(shootDirection));

        ProjectileData data = new(projectile.netId)
        {
            SenderId = sender.netId,
            Direction = shootDirection
        };

        projectile.Initialize(data);

        return projectile;
    }
}

