using System;
using UnityEngine;

[Serializable]
public class NonMovableMethod : ProjectileMovementMethod
{
    protected override void OnMove(Projectile self, Vector3 velocity) { return; }
}

