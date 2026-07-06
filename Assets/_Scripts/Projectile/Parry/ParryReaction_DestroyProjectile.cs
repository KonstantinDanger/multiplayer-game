using System;
using UnityEngine;

[Serializable]
public class ParryReaction_DestroyProjectile : ProjectileParryReaction
{
    public override void ReactTo(GameObject parrySender)
    {
        if (Projectile.Data.Sender == parrySender)
            return;

        GameObject.Destroy(Projectile.gameObject);
    }
}