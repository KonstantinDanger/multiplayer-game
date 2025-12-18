using Mirror;
using UnityEngine;

public class ProjectileFactory
{
    public Projectile Create(Projectile prefab, Transform shootPosition, Vector3 shootDirection, NetworkBehaviour sender)
    {
        var projectile = Object.Instantiate(prefab, shootPosition.position, Quaternion.LookRotation(shootDirection));

        ProjectileData data = new()
        {
            SenderId = sender.netId,
            Direction = shootDirection
        };

        projectile.Initialize(data);

        return projectile;
    }
}

