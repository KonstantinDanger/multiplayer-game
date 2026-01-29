using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/MoveToTargetAction", fileName = "MoveToTargetAction")]
public class MoveToTargetAction : AIAction
{
    private Enemy _enemy;

    public override void Initialize(NetworkBehaviour self)
        => _enemy = self.GetComponent<Enemy>();

    public override void Execute(NetworkBehaviour self, NetworkBehaviour target)
    {
        _enemy.Movable.Move(target.transform.position, _enemy.MovementConfig.Speed);
        _enemy.Rotatable?.Rotate((target.transform.position - self.transform.position).normalized, _enemy.RotationConfig.RotationSpeed);
    }
}
