using Mirror;
using System.Collections.Generic;

public class GameMatchSummaryState : GameState
{
    private StaticData _staticData;
    private CustomNetworkManager _netManager;
    private List<Player> _players = new();
    private GameMatchData _matchData;
    private GameMatchConfig _gameMatchConfig;

    private float _timer;
    private bool _ended;

    public GameMatchSummaryState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _matchData = Resolve<PersistentGameData>().GameData.GameMatchData;

        _staticData = Resolve<StaticData>();
        _netManager = Resolve<CustomNetworkManager>();
        _gameMatchConfig = _staticData.GameMatchConfig;

        _players = GetPlayers();
        DisablePlayers();
        ShowOutcome(GameMatchState.Outcome);
        RewardPlayers();
        BackupPlayerData();

        //BackendApi.CreateMatch(matchData);

        _netManager.OnServerSceneLoaded += HandleLobbySceneLoaded;

        _ended = false;
    }

    public override void OnExit()
    {
        _netManager.OnServerSceneLoaded -= HandleLobbySceneLoaded;
        _timer = 0f;
    }

    public override void Update(float deltaTime)
    {
        if (_ended)
            return;

        _timer += deltaTime;

        if (_timer >= _staticData.GameMatchConfig.MatchSummaryDuration)
        {
            _netManager.ServerChangeScene(_staticData.LobbySceneName);
            _ended = true;
        }
    }

    private void RewardPlayers()
    {
        var outcome = GameMatchState.Outcome;

        switch (outcome)
        {
            case MatchResult.None:
            case MatchResult.CanceledEarly:
                return;
        }

        var loser = GetPlayerForSteamName(_matchData.MatchData.loserSteamName);
        var winner = GetPlayerForSteamName(_matchData.MatchData.winnerSteamName);

        UnityEngine.Debug.Log("outcome " + outcome);
        switch (outcome)
        {

            case MatchResult.OneSided:
            case MatchResult.Surrender:
                RewardPlayer(winner, _gameMatchConfig.WinReward);
                RewardPlayer(loser, _gameMatchConfig.LoseReward);
                break;

            case MatchResult.Draw:
                _players.ForEach(p => RewardPlayer(p, _gameMatchConfig.DrawReward));
                break;
        }
    }

    private void BackupPlayerData()
        => _players.ForEach(p => p.SaveData());

    private Player GetPlayerForSteamName(string steamName)
    {
        foreach (Player player in _players)
            if (player.SteamName == steamName)
                return player;

        return null;
    }

    private void RewardPlayer(Player player, int reward)
    {
        if (player.TryGetComponent(out Wallet wallet))
        {
            wallet.Add(CurrencyType.Meta, reward);
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
        float summaryDuration = _staticData.GameMatchConfig.MatchSummaryDuration;
        _players.ForEach(player =>
        {
            player.ShowMatchOutcome(outcome, _matchData.MatchData, summaryDuration);
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
    {
        _players.ForEach(player =>
        {
            player?.TargetRpcToggleMatchHUD(false);
            player?.HideMatchOutcome();
        });
    }

    private void DisablePlayers()
    {
        _players.ForEach(player =>
        {
            player.SetCanAttack(false);
            player.Spectate(true);
        });
    }
}
