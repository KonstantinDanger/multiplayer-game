using Mirror;
using UnityEngine;

public class NetworkManagerLobbyHUD : NetworkManagerHUD
{
    [SerializeField] private CustomNetworkManager _netManager;

    protected override void OnGUI()
    {
        if (NetworkServer.active)
            if (GUILayout.Button("Start Game"))
                HandleStartGame();

        base.OnGUI();
    }

    private void HandleStartGame()
    {
        var staticData = ServiceLocator.Container.Resolve<StaticData>();

        _netManager.ServerChangeScene(staticData.GameSceneName);
    }
}
