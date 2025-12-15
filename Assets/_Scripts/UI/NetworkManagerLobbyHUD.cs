using Mirror;
using UnityEngine;

public class NetworkManagerLobbyHUD : NetworkManagerHUD
{
    [SerializeField] private CustomNetworkManager _netManager;

    protected override void OnGUI()
    {
        if (NetworkServer.active)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // 2. Add flexible space to consume all space before the button.
            GUILayout.FlexibleSpace();

            // 3. Render the button. You might also want to limit its width 
            //    so it doesn't try to fill the entire 300px area. (Optional)
            if (GUILayout.Button("Start Game", GUILayout.Width(120))) // Adjust width as needed
                HandleStartGame();

            // 4. End the container
            GUILayout.EndHorizontal();
        }

        base.OnGUI();
    }

    private void HandleStartGame()
    {
        var staticData = ServiceLocator.Container.Resolve<StaticData>();

        _netManager.ServerChangeScene(staticData.GameSceneName);
    }
}
