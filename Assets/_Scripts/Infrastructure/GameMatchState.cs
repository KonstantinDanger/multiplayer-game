using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMatchState : GameState
{
    private readonly List<Player> _players = new();
    private readonly List<PlayerSummaryDataBinder> _dataBinders = new();

    private GameMatchConfig _gameMatchData;
    private GameFactory _factory;
    private StaticData _data;

    private Zone _zone;
    private Match _match;
    private GameData _gameData;

    private float _matchTime;

    public GameMatchState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _data = Resolve<StaticData>();
        _factory = Resolve<GameFactory>();
        _gameMatchData = _data.GameMatchData;
        _gameData = Resolve<PersistentGameData>().GameData;

        InitZone();
        InitMatch();
        InitPlayers(_match);
        InitializeMatchData(_gameData);

        Events.OnPlayerLost += HandlePlayerLost;
    }

    public override void Update(float deltaTime)
    {
        _match.Update(deltaTime);

        float matchProgress = _match.GetMatchProgress();

        _zone.Shrink(matchProgress);

        var playersOutOfZone = GetPlayersOutOfZone(_players);

        TryDamagePlayersOutOfZone(playersOutOfZone, _data.ZoneDPS * deltaTime);

        _matchTime += deltaTime;
    }

    public override void OnExit()
    {
        Events.OnPlayerLost -= HandlePlayerLost;

        _dataBinders
            .ForEach(dataB => dataB
            .Unbind());

        _players.ForEach(player =>
        {
            player.ToggleHUD(false);
            player.SetCanAttack(false);
            player.Dispose();
        });
    }

    private void HandlePlayerLost(Player player)
    {
        var loser = player;
        var winner = _players.Find(p => p != player);

        PlayerMatchSummaryData[] _summaries = _players
            .Select(player => _gameData.GameMatchData
            .GetSummaryFor(player))
            .ToArray();

        _gameData.GameMatchData.MatchData = new()
        {
            winner = winner.name,
            loser = loser.name,
            matchTime = _matchTime
        };

        _gameData.GameMatchData.MatchData.SetDate(DateTime.Now);

        GoTo<GameMatchSummaryState>();
    }

    private void InitializeMatchData(GameData gameData)
    {
        foreach (var player in _players)
        {
            PlayerData data = new()
            {
                Name = player.name, //Steam player name
                Currency = 0, //Fetch currency from server
                UnlockedClasses = new[] { "Light mage" } //Fetch classes from server 
            };

            PlayerMatchSummaryData summaryData = gameData.GameMatchData.AddNewSummaryForPlayerData(data);

            PlayerSummaryDataBinder binder = new(player, summaryData);

            binder.Bind();

            _dataBinders.Add(binder);
        }
    }

    private void InitPlayers(Match match)
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            Player player = conn.identity.GetComponent<Player>();
            player.Initialize(match);
            player.ResetLevel();
            player.ToggleHUD(true);
            player.SetCanAttack(true);
            _players.Add(player);
        }

        Resolve<CustomNetworkManager>()
            .ShufflePlayersPositions();
    }

    private void InitMatch()
    {
        _match = new(_gameMatchData, _players);

        _match.Start();
    }

    private void InitZone()
        => _zone = _factory.SpawnZone(_data.ZonePrefab, Vector3.zero);

    private void TryDamagePlayersOutOfZone(IEnumerable<Player> playersOutOfZone, float damage)
    {
        if (playersOutOfZone.Count() == 0)
            return;

        foreach (Player player in playersOutOfZone)
            player.Damageable.TakeDamage(new Damage() { Amount = damage, Type = DamageType.Absolute });
    }

    private IEnumerable<Player> GetPlayersOutOfZone(IEnumerable<Player> players)
        => players?.Where(player => Vector3.Distance(player.transform.position, _zone.transform.position) > _zone.Radius);
}
