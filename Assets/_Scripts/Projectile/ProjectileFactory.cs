using UnityEngine;

public class ProjectileFactory
{
    public Projectile Create(Projectile prefab, Transform shootPosition, Vector3 shootDirection, GameObject sender)
    {
        var projectile = Object.Instantiate(prefab, shootPosition.position, Quaternion.LookRotation(shootDirection));

        ProjectileData data = new()
        {
            Sender = sender,
            Direction = shootDirection
        };

        projectile.Initialize(data);

        return projectile;
    }
}

