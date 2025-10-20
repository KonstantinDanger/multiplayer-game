using System;
using System.Collections.Generic;

public class Match : IMatch
{
    public event Action OnStarted;
    public event Action OnEnded;

    private readonly GameMatchData _data;

    private readonly Timer _matchTimer;
    private readonly Timer _deathMatchTimer;

    private readonly List<Player> _players;

    public Match(GameMatchData data, IEnumerable<Player> players)
    {
        _data = data;
        _matchTimer = new(_data.MatchTime * Constants.SecondsInMinute);
        _deathMatchTimer = new(_data.DeathmatchTime * Constants.SecondsInMinute);
        //_players = players.ToList();
    }

    public float GetMatchProgress()
        => _matchTimer.ElapsedTime / _data.MatchTime;

    public void Start()
    {
        if (_matchTimer.IsEnded)
            return;

        _matchTimer.Start();
    }

    public void Update(float deltaTime)
    {
        if (!_matchTimer.IsEnded)
        {
            _matchTimer.Update(deltaTime);
        }
        else
        {
            if (!_deathMatchTimer.IsEnded)
            {
                if (!_deathMatchTimer.HasStarted)
                    StartDeathMatch();

                _deathMatchTimer.Update(deltaTime);
            }
        }
    }

    private void StartDeathMatch()
        => _deathMatchTimer.Start();
}

