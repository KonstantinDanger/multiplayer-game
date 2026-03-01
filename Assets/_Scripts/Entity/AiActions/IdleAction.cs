using Mirror;
using System;

[Serializable]
public class IdleAction : AIAction
{
    public override void Execute(Enemy self, NetworkBehaviour target)
        => self.Movable.Move(self.transform.position, 0f);
}
