using Mirror;

public class GameMatchSummaryState : GameState
{
    private const float SummaryTime = 5;
    private float _timer;
    private StaticData _staticData;
    private CustomNetworkManager _netManager;

    public GameMatchSummaryState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    private bool _ended;

    public override void OnEnter()
    {
        DisablePlayers();

        var matchData = Resolve<PersistentGameData>().GameData.GameMatchData;

        _staticData = Resolve<StaticData>();
        _netManager = Resolve<CustomNetworkManager>();

        //Send request to backend to save the data

        UnityEngine.Debug.Log("matchData " + matchData.ToString());

        _netManager.OnServerSceneLoaded += HandleLobbySceneLoaded;

        _ended = false;
    }

    private void HandleLobbySceneLoaded(string sceneName)
    {
        if (sceneName != _staticData.LobbySceneName)
            throw new System.Exception("There was an error during lobby scene load. Possibly, the wrong scene has loaded ");

        GoTo<GameLobbyRefreshState>();
    }

    public override void Update(float deltaTime)
    {
        if (_ended)
            return;

        _timer += deltaTime;

        if (_timer >= SummaryTime)
        {
            _netManager.ServerChangeScene(_staticData.LobbySceneName);
            _ended = true;
        }
    }

    private void DisablePlayers()
    {
        foreach (var item in NetworkServer.connections)
        {
            Player player = item.Value.identity.GetComponent<Player>();
            player.SetCanAttack(false);
            player.Spectate(true);
        }
    }
}
