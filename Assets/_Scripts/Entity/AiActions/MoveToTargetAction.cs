using Mirror;
using System;
using UnityEngine;

[Serializable]
public class MoveToTargetAction : AIAction
{
    private CapsuleCollider _collider;

    private INavigationBasedMover _navigationMover;

    public override void Initialize(NetworkBehaviour self)
    {
        _collider = self.GetComponent<CapsuleCollider>();

        if (!self.TryGetComponent(out _navigationMover))
            throw new Exception("No direction mover found!");
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        float distanceToTarget = Vector3.Distance(self.transform.position, target.transform.position);

        if (distanceToTarget <= _collider.radius)
            return;

        _navigationMover.Move(target.transform.position, self.MovementConfig.Speed);
        self.Rotatable?.Rotate((target.transform.position - self.transform.position).normalized, self.RotationConfig.RotationSpeed);
    }
}
