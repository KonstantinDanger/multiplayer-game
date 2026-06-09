using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Supposed to be a server state
/// </summary>
public class GameMatchState : GameState
{
    public static MatchResult Outcome = MatchResult.None;

    private readonly List<Player> _players = new();
    private readonly List<PlayerSummaryDataBinder> _dataBinders = new();

    private GameMatchConfig _gameMatchConfig;
    private GameFactory _factory;
    private StaticData _staticData;

    private Zone _zone;
    private Match _match;
    private GameData _gameData;

    private Timer _drawCountdown;
    private float _matchTime;
    private CustomNetworkManager _netManager;

    public GameMatchState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        Outcome = MatchResult.None;

        _staticData = Resolve<StaticData>();
        _factory = Resolve<GameFactory>();
        _gameMatchConfig = _staticData.GameMatchConfig;
        _gameData = Resolve<PersistentGameData>().GameData;
        _netManager = Resolve<CustomNetworkManager>();

        _drawCountdown = new(_gameMatchConfig.DrawCountdown);

        InitSpawnPositions();
        InitZone();
        InitMatch();
        InitializeMatchData(_gameData);
        InitPlayersForConnections();

        _match.OnDeathmatchStarted += HandleDeathmatchStarted;
        Events.ServerOnPlayerDemise += HandlePlayerDemise;
        Events.ServerOnMatchLeaveRequest += HandlePlayerLeaveRequest;

        NotifyPlayersMatchStatusChanged(true);
    }

    public override void Update(float deltaTime)
    {
        _match.Update(deltaTime);

        float matchProgress = _match.GetMatchProgress();

        _zone.Shrink(matchProgress);

        var playersOutOfZone = GetPlayersOutOfZone(_players);

        TryDamagePlayersOutOfZone(playersOutOfZone, _staticData.ZoneDPS * deltaTime);

        _drawCountdown.Update(deltaTime);

        _matchTime += deltaTime;

        UpdatePlayersMatchHUDs(matchProgress);
    }

    public override void OnExit()
    {
        Events.ServerOnPlayerDemise -= HandlePlayerDemise;
        Events.ServerOnMatchLeaveRequest -= HandlePlayerLeaveRequest;

        _drawCountdown.Stop();

        _dataBinders
            .ForEach(dataB => dataB
            .Unbind());

        _players.ForEach(player =>
        {
            player.SetCanAttack(false);
            player.Dispose();
            player.TargetSetLobbyHUDActive(true);
        });

        NotifyPlayersMatchStatusChanged(false);

        _matchTime = 0f;
        _players.Clear();
    }

    #region Match Outcome

    private void HandlePlayerLeaveRequest(uint playerId)
    {
        Player leavedOne = Utils.ServerFindNetPlayerById(playerId);

        if (_match.GetMatchProgress() < _gameMatchConfig.CancellationThreshold)
            HandleMatchCancel();
        else
            HandleSurrenderFor(leavedOne);
    }

    private void HandlePlayerDemise(uint playerId)
    {
        Player player = Utils.ServerFindNetPlayerById(playerId);

        if (_match.IsDeathmatchActive)
        {
            if (!_drawCountdown.HasStarted)
                _drawCountdown.Start(onEnded: () => HandlePlayerMatchLostFor(player));
            else
                HandleDraw();

            return;
        }

        player.Respawn();
    }

    private void HandleMatchCancel()
    {
        SetSummaries();

        _gameData.GameMatchData.MatchData = new() { matchTime = _matchTime };

        _gameData.GameMatchData.MatchData.SetDate(DateTime.Now);

        SetOutcome(MatchResult.CanceledEarly);

        GoTo<GameMatchSummaryState>();
    }

    private void HandleSurrenderFor(Player player)
    {
        HandlePlayerMatchLostFor(player);
        SetOutcome(MatchResult.Surrender);
    }

    private void HandleDraw()
    {
        SetSummaries();

        _gameData.GameMatchData.MatchData = new() { matchTime = _matchTime };

        _gameData.GameMatchData.MatchData.SetDate(DateTime.Now);

        SetOutcome(MatchResult.Draw);

        GoTo<GameMatchSummaryState>();
    }

    private void SetSummaries()
    {
        PlayerMatchSummaryData[] _summaries = _players
            .Select(player => _gameData.GameMatchData
            .GetSummaryFor(player))
            .ToArray();
    }

    private void SetOutcome(MatchResult result)
        => Outcome = result;

    private void HandlePlayerMatchLostFor(Player player)
    {
        var loser = player;
        var winner = _players.Find(p => p != player);

        SetSummaries();

        _gameData.GameMatchData.MatchData = new()
        {
            winnerSteamName = winner.SteamName,
            loserSteamName = loser.SteamName,
            matchTime = _matchTime
        };

        _gameData.GameMatchData.MatchData.SetDate(DateTime.Now);

        SetOutcome(MatchResult.OneSided);

        GoTo<GameMatchSummaryState>();
    }

    #endregion

    private void UpdatePlayersMatchHUDs(float progress)
        => _players
        .ForEach(p => p
        .UpdateMatchHUD(_matchTime, progress, _match.IsDeathmatchActive));

    private void NotifyPlayersMatchStatusChanged(bool active)
    {
        _players.ForEach(p =>
        {
            if (p.TryGetComponent(out MatchStatusReceiver statusReceiver))
                statusReceiver.RequestMatchStatusChange(active);
        });
    }

    private void InitSpawnPositions()
    {
        List<Transform> spawnPoints = UnityEngine.Object
            .FindObjectsByType<NetworkStartPosition>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .ToList()
            .Select(p => p.transform)
            .ToList();

        _netManager.SetSpawnPositions(spawnPoints);
    }

    private void InitializeMatchData(GameData gameData)
    {
        foreach (var player in _players)
        {
            PlayerData data = new()
            {
                Name = player.SteamName, //Steam player name
                MetaCurrency = 0, //Fetch currency from server
                UnlockedClasses = new[] { "Light mage" } //Fetch classes from server 
            };

            PlayerMatchSummaryData summaryData = gameData.GameMatchData.AddNewSummaryForPlayerData(data);
            Wallet wallet = player.GetComponent<Wallet>();
            wallet.ResetCurrency(CurrencyType.Match);
            PlayerSummaryDataBinder binder = new(player, summaryData, wallet);

            binder.Bind();

            _dataBinders.Add(binder);
        }
    }

    private void InitPlayersForConnections()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            Player player = conn.identity.GetComponent<Player>();
            player.ResetLevel();
            player.Damageable.Respawn();
            player.SetCanAttack(true);
            player.TargetRpcToggleMatchHUD(true);
            player.TargetSetLobbyHUDActive(false);
            player.GetComponent<AnimatorUpdater>()
                 .Initialize();
            _players.Add(player);
        }

        _netManager.ShufflePlayersPositions();
    }

    private void InitMatch()
    {
        _match = new(_gameMatchConfig, _players);

        _match.Start();
    }

    private void InitZone()
    {
        Map map = Resolve<Map>(resolveCached: true);

        _zone = _factory.SpawnZone(_staticData.ZonePrefab, map.MapCenter.position);
    }

    private void HandleDeathmatchStarted()
    {
        //Notify players to show corresponding ui or logic
    }

    private void TryDamagePlayersOutOfZone(IEnumerable<Player> playersOutOfZone, float damage)
    {
        if (playersOutOfZone.Count() == 0)
            return;

        foreach (Player player in playersOutOfZone)
            player?.Damageable.TakeDamage(new Damage() { Amount = damage, Type = DamageType.Absolute });
    }

    private IEnumerable<Player> GetPlayersOutOfZone(IEnumerable<Player> players)
        => players?.Where(player => Vector3.Distance(player.transform.position, _zone.transform.position) > _zone.Radius);
}
