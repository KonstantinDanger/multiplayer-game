using Mirror;
using System;

[Serializable]
public class MoveToTargetAction : AIAction
{
    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        self.Movable.Move(target.transform.position, self.MovementConfig.Speed);
        self.Rotatable?.Rotate((target.transform.position - self.transform.position).normalized, self.RotationConfig.RotationSpeed);
    }
}
