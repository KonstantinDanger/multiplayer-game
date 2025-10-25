using Mirror;
using UnityEngine;

public class GameFactory : NetworkBehaviour
{
    private MainUI _mainUI;

    //[Command(requiresAuthority = false)]
    //public Player CmdSpawnPlayer(Player player, Transform point)
    //{
    //    Player playerObject = Object.Instantiate(player, point.position, Quaternion.identity);

    //    Object.DontDestroyOnLoad(playerObject);

    //    NetworkServer.AddPlayerForConnection(playerObject.connectionToClient, playerObject.gameObject);

    //    return playerObject;
    //}

    public MainUI InitializeUI(MainUI UIPrefab, Transform transform)
    {
        _mainUI = Instantiate(UIPrefab, transform);
        DontDestroyOnLoad(_mainUI);
        return _mainUI;
    }

    public UI AddUI(UI ui)
        => Instantiate(ui, _mainUI.transform);

    //public void RemoveUI(UI ui)
    //{
    //    var target = _mainUI.transform.Find(ui.name);

    //    if (!target)
    //        return;

    //    Object.Destroy(target.gameObject);
    //}

    public CustomNetworkManager CreateNetworkManager(CustomNetworkManager networkManagerPrefab, Transform transform)
        => Instantiate(networkManagerPrefab, transform);

    [Server]
    public Zone SpawnZone(Zone zonePrefab, Vector3 position)
    {
        var zone = Instantiate(zonePrefab, position, Quaternion.identity);
        NetworkServer.Spawn(zone.gameObject);
        return zone;
    }

    public Player SpawnPlayer(NetworkConnectionToClient conn, Player playerPrefab, Vector3 position)
    {
        Player player = Instantiate(playerPrefab, position, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player.gameObject);

        return player;
    }
}
