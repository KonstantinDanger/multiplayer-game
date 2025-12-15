using Mirror;
using System.Collections;
using UnityEngine;

public class Respawn : NetworkBehaviour
{
    [Command(requiresAuthority = false)]
    public void Execute(uint playerId, float respawnTime)
        => ServerExecute(playerId, respawnTime);

    [Server]
    private void ServerExecute(uint playerId, float respawnTime)
        => StartCoroutine(RespawnRoutine(playerId, respawnTime));

    private IEnumerator RespawnRoutine(uint playerId, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        EnablePlayer(playerId);
    }

    [ClientRpc]
    private void EnablePlayer(uint playerId)
    {
        if (!NetworkClient.spawned.TryGetValue(playerId, out NetworkIdentity identity))
            throw new System.Exception("Could not resolve a player");

        CustomNetworkManager netMan = ServiceLocator.Container.Resolve<CustomNetworkManager>();
        var spawnPos = netMan.GetStartPosition();

        Player player = identity.GetComponent<Player>();
        player.Spectate(false);
        player.RefillHealth();
        CmdWarpPlayer(playerId, spawnPos.position);
    }

    [Command]
    private void CmdWarpPlayer(uint playerId, Vector3 position)
        => ServerWarpPlayer(playerId, position);

    [Server]
    private void ServerWarpPlayer(uint playerId, Vector3 position)
    {
        var player = Utils.ServerFindNetPlayerById(playerId);
        player.Movable.Warp(position);
    }
}