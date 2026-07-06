using Mirror;
using UnityEngine;

[System.Serializable]
public abstract class ProjectileParryReaction
{
    [SerializeField] protected Projectile Projectile;

    public abstract void ReactTo(NetworkBehaviour parrySender);
}

