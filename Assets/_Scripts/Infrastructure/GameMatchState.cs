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
    private List<Player> _players = new();

    public GameMatchState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _data = Resolve<StaticData>();
        _factory = Resolve<GameFactory>();
        _gameMatchData = _data.GameMatchData;

        foreach (var conn in NetworkServer.connections.Values)
        {
            Player player = conn.identity.GetComponent<Player>();
            _players.Add(player);
        }

        Resolve<CustomNetworkManager>().ShufflePlayersPositions();

        _zone = _factory.SpawnZone(_data.ZonePrefab, Vector3.zero);

        _match = new(_gameMatchData, _players);

        _match.Start();
    }

    public override void Update(float deltaTime)
    {
        _match.Update(deltaTime);

        float matchProgress = _match.GetMatchProgress();

        _zone.Shrink(matchProgress);

        const float zoneDamage = 3;

        var playersOutOfZone = GetPlayersOutOfZone(_players);

        TryDamagePlayersOutOfZone(playersOutOfZone, zoneDamage * deltaTime);
    }

    private void TryDamagePlayersOutOfZone(IEnumerable<Player> playersOutOfZone, float damage)
    {
        if (playersOutOfZone.Count() == 0)
            return;

        foreach (Player player in playersOutOfZone)
            player.Damageable.TakeDamage(new Damage() { Amount = damage });
    }

    private IEnumerable<Player> GetPlayersOutOfZone(IEnumerable<Player> players)
        => players.Where(player => Vector3.Distance(player.transform.position, _zone.transform.position) > _zone.Radius);
}
