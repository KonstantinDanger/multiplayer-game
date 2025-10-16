using UnityEngine;

public class PlayersInfo : MonoBehaviour
{
    [SerializeField] private Transform _infoHolder;
    [SerializeField] private PlayerInfoUI _playerInfoUIPrefab;

    public void AddInfo(ulong steamId)
    {
        _playerInfoUIPrefab = Instantiate(_playerInfoUIPrefab, _infoHolder);

        _playerInfoUIPrefab.SetSteamId(steamId);
    }
}
