using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class ProjectileCollisionReaction
{
    public UnityEvent OnCollided;

    [Header("On after collided")]
    [SerializeReference, SubclassSelector]
    private AfterCollision _afterCollision;

    public void Collide(Collider collider, Projectile self)
    {
        OnCollide(collider, self);

        if (_afterCollision == null)
        {
            UnityEngine.Object.Destroy(self.gameObject);
            return;
        }

        OnCollided?.Invoke();
        _afterCollision?.PerformAfterCollision(collider, self);
    }

    protected abstract void OnCollide(Collider collider, Projectile self);
}