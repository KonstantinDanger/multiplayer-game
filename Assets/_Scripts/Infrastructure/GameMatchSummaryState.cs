using Mirror;
using System.Collections.Generic;

public class GameMatchSummaryState : GameState
{
    private float _timer;
    private StaticData _staticData;
    private CustomNetworkManager _netManager;
    private List<Player> _players = new();

    public GameMatchSummaryState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    private bool _ended;

    public override void OnEnter()
    {
        GameMatchData matchData = Resolve<PersistentGameData>().GameData.GameMatchData;

        _staticData = Resolve<StaticData>();
        _netManager = Resolve<CustomNetworkManager>();

        _players = GetPlayers();
        DisablePlayers();
        ShowOutcome(GameMatchState.Outcome);

        //BackendApi.CreateMatch(matchData);

        _netManager.OnServerSceneLoaded += HandleLobbySceneLoaded;

        _ended = false;
    }

    public override void OnExit()
        => _netManager.OnServerSceneLoaded -= HandleLobbySceneLoaded;

    public override void Update(float deltaTime)
    {
        if (_ended)
            return;

        _timer += deltaTime;

        if (_timer >= _staticData.GameMatchData.MatchSummaryDuration)
        {
            _netManager.ServerChangeScene(_staticData.LobbySceneName);
            _ended = true;
        }
    }

    private List<Player> GetPlayers()
    {
        List<Player> players = new();

        foreach (var item in NetworkServer.connections)
        {
            if (item.Value.identity.TryGetComponent(out Player player))
                players.Add(player);
        }

        return players;
    }

    private void ShowOutcome(MatchResult outcome)
    {
        float summaryDuration = _staticData.GameMatchData.MatchSummaryDuration;
        _players.ForEach(player =>
        {
            player.ShowMatchOutcome(outcome, summaryDuration);
        });
    }

    private void HandleLobbySceneLoaded(string sceneName)
    {
        if (sceneName != _staticData.LobbySceneName)
            throw new System.Exception("There was an error during lobby scene load. Possibly, the wrong scene has loaded ");

        HideSummaryAndHUD();
        _netManager.ShufflePlayersPositions();
        GoTo<GameLobbyRefreshState>();
    }

    private void HideSummaryAndHUD()
        => _players.ForEach(player =>
        {
            player?.TargetRpcToggleHUD(false);
            player?.HideMatchOutcome();
        });

    private void DisablePlayers()
    {
        _players.ForEach(player =>
        {
            player.SetCanAttack(false);
            player.Spectate(true);
        });
    }
}
