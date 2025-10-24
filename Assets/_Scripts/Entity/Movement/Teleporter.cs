using Mirror;
using UnityEngine;

public class Teleporter : NetworkBehaviour
{
    public void Warp(GameObject gameObject, Vector3 position)
        => CmdWarpPlayer(gameObject, position);

    [Command]
    private void CmdWarpPlayer(GameObject gameObject, Vector3 position)
        => gameObject.GetComponent<IMovable>().Warp(position);
}
