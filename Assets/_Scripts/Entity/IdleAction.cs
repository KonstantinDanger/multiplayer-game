using Mirror;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/IdleAction", fileName = "IdleAction")]
public class IdleAction : AIAction
{
    private IMovable _movable;

    public override void Initialize(NetworkBehaviour self)
        => _movable = self.GetComponent<IMovable>();

    public override void Execute(NetworkBehaviour self, NetworkBehaviour target)
        => _movable.Move(self.transform.position, 0f);
}
