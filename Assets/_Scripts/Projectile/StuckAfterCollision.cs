using System;
using UnityEngine;

[Serializable]
public class StuckAfterCollision : AfterCollision
{
    public override void PerformAfterCollision(Collider collidedWith, Projectile self)
    {
        ref ProjectileData data = ref self.Data;
        Transform projectileTransform = data.Collider.transform;

        Vector3 closestPoint = collidedWith.ClosestPointOnBounds(projectileTransform.position);

        data.Speed = 0;
        data.Collider.enabled = false;

        projectileTransform.position = closestPoint;
        projectileTransform.SetParent(collidedWith.transform);
        Vector3 lookAtDirection = collidedWith.transform.position - projectileTransform.position;
        projectileTransform.rotation = Quaternion.LookRotation(data.Direction);
    }
}