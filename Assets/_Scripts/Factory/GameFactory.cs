using UnityEngine;

public class GameFactory
{
    private MainUI _mainUI;

    public Player SpawnPlayer(Player player, Transform point)
        => Object.Instantiate(player, point.position, Quaternion.identity);

    public MainUI InitializeUI(MainUI UIPrefab, Transform transform)
    {
        _mainUI = Object.Instantiate(UIPrefab, transform);
        GameObject.DontDestroyOnLoad(_mainUI);
        return _mainUI;
    }

    public UI AddUI(UI ui)
        => Object.Instantiate(ui, _mainUI.transform);

    //public void RemoveUI(UI ui)
    //{
    //    var target = _mainUI.transform.Find(ui.name);

    //    if (!target)
    //        return;

    //    Object.Destroy(target.gameObject);
    //}

    public CustomNetworkManager CreateNetworkManager(CustomNetworkManager networkManagerPrefab, Transform transform)
        => Object.Instantiate(networkManagerPrefab, transform);

    public Zone SpawnZone(Zone zonePrefab, Vector3 position)
        => Object.Instantiate(zonePrefab, position, Quaternion.identity);
}
