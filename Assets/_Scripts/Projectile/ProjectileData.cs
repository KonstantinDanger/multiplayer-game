using UnityEngine;

public struct ProjectileData
{
    public uint ProjectileId { get; private set; }

    public float Speed;
    public uint SenderId;
    public Vector3 Direction;
    public Damage Damage;

    public readonly Collider Collider
    {
        get
        {
            Projectile projectile = Utils
                .NetIdToGameObject(ProjectileId)
                .GetComponent<Projectile>();

            return projectile.Collider;
        }
    }

    public readonly GameObject Sender => Utils.NetIdToGameObject(SenderId);

    public ProjectileData(uint projectileId)
    {
        Speed = 0;
        SenderId = default;
        Direction = Vector3.zero;
        ProjectileId = projectileId;
        Damage = new();
    }
}
