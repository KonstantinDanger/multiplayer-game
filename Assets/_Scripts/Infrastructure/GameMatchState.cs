using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMatchState : GameState
{
    private GameMatchData _gameMatchData;
    private GameFactory _factory;
    private StaticData _data;

    private Zone _zone;
    private Match _match;
    private List<Player> _players;

    public GameMatchState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container) { }

    public override void OnEnter()
    {
        _players = Object.FindObjectsByType<Player>(FindObjectsSortMode.None).ToList(); //TODO: Make more reliable way to get players

        _data = Resolve<StaticData>();
        _factory = Resolve<GameFactory>();
        _gameMatchData = _data.GameMatchData;

        _zone = _factory.SpawnZone(_data.ZonePrefab, Vector3.zero);
        _match = new(_gameMatchData, _players);

        _match.Start();
    }

    public override void Update(float deltaTime)
    {
        _match.Update(deltaTime);

        float matchProgress = _match.GetMatchProgress();

        _zone.Shrink(matchProgress);

        var playersOutOfZone = GetPlayersOutOfZone(_players);

        const float zoneDamage = 10;

        if (playersOutOfZone.Count() > 0)
        {
            foreach (Player player in playersOutOfZone)
            {
                UnityEngine.Debug.Log($"player {player.name} takes {zoneDamage * deltaTime} damage");
            }
        }
    }

    private IEnumerable<Player> GetPlayersOutOfZone(IEnumerable<Player> players)
        => players.Where(player => Vector3.Distance(player.transform.position, _zone.transform.position) > _zone.CurrentRadius);
}
