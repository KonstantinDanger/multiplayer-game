using Mirror;
using UnityEngine;

[System.Serializable]
public abstract class ProjectileParryReaction
{
    [SerializeField] protected Projectile Projectile;

    public void ReactTo(NetworkBehaviour parrySender)
    {
        ProjectileData data = Projectile.Data;

        if (data.Sender == parrySender)
            return;

        if (parrySender.TryGetComponent(out Entity senderEntity))
        {
            if (data.Damage.TeamId == senderEntity.TeamId)
                return;

            Projectile.Data.Damage.TeamId.Id = senderEntity.TeamId.Id;
        }

        OnReactTo(parrySender);
    }

    protected abstract void OnReactTo(NetworkBehaviour parrySender);
}

