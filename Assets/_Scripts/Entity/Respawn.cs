using Mirror;
using System.Collections;
using UnityEngine;

public class Respawn : NetworkBehaviour
{
    CustomNetworkManager _networkManager;

    private void Start()
        => _networkManager = ServiceLocator.Container.Resolve<CustomNetworkManager>();

    public void Execute(Player player, float respawnTime)
        => StartCoroutine(RespawnRoutine(player, respawnTime));

    [Server]
    private IEnumerator RespawnRoutine(Player player, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);
        EnablePlayer(player);
    }

    [ClientRpc]
    private void EnablePlayer(Player player)
    {
        player.Spectate(false);
        var spawnPos = _networkManager.GetStartPosition();
        player.Movable.Warp(spawnPos.position);
        player.Respawn();
    }
}