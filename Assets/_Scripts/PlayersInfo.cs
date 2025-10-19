using Mirror;
using UnityEngine;

public class PlayersInfo : NetworkBehaviour
{
    [SerializeField] private PlayerInfoCell _playerInfoCellPrefab;

    [Server]
    public void RpcAddInfo(PlayerInfoUI infoUI, ulong steamId, NetworkConnectionToClient conn)
    {
        var infoCell = Instantiate(_playerInfoCellPrefab, infoUI.Container.transform);

        infoCell.SetSteamId(steamId);
    }
}
