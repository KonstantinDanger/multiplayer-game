using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMatchState : GameState
{
    private GameMatchConfig _gameMatchData;
    private GameFactory _factory;
    private StaticData _data;

    private Zone _zone;
    private Match _match;
    private readonly List<Player> _players = new();

    public GameMatchState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _data = Resolve<StaticData>();
        _factory = Resolve<GameFactory>();
        _gameMatchData = _data.GameMatchData;

        InitZone();
        InitMatch();
        InitPlayers(_match);
    }

    public override void Update(float deltaTime)
    {
        _match.Update(deltaTime);

        float matchProgress = _match.GetMatchProgress();

        _zone.Shrink(matchProgress);

        var playersOutOfZone = GetPlayersOutOfZone(_players);

        TryDamagePlayersOutOfZone(playersOutOfZone, _data.ZoneDPS * deltaTime);
    }

    public override void OnExit()
    {
        _players.ForEach(player =>
        {
            player.SetCanAttack(false);
            player.Dispose();
        });
    }

    private void InitPlayers(Match match)
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            Player player = conn.identity.GetComponent<Player>();
            player.Initialize(match);
            player.SetCanAttack(true);
            _players.Add(player);
        }

        Resolve<CustomNetworkManager>().ShufflePlayersPositions();
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
        => players.Where(player => Vector3.Distance(player.transform.position, _zone.transform.position) > _zone.Radius);
}
