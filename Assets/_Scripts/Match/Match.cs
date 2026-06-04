using System;
using System.Collections.Generic;

public class Match : IMatch
{
    public event Action OnStarted;
    public event Action OnEnded;
    public event Action OnDeathmatchStarted;

    private readonly GameMatchConfig _data;

    private readonly Timer _matchTimer;
    private readonly Timer _deathMatchTimer;

    private readonly float _matchTime;
    private readonly float _deathMatchTime;

    public bool IsDeathmatchActive { get; private set; }
    public float ElapsedTimeSinceStart => _matchTimer.ElapsedTime;

    public Match(GameMatchConfig data, IEnumerable<Player> players)
    {
        _data = data;

        _matchTime = _data.MatchTime * StaticData.Constants.SecondsInMinute;
        _deathMatchTime = _data.DeathmatchTime * StaticData.Constants.SecondsInMinute;

        _matchTimer = new(_matchTime);
        _deathMatchTimer = new(_deathMatchTime);
    }

    /// <summary>
    /// Returns normalized match progress [0.0, 1.0]
    /// </summary>
    /// <returns></returns>
    public float GetMatchProgress()
        => _matchTimer.ElapsedTime / _matchTime;

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
    {
        _deathMatchTimer.Start();
        IsDeathmatchActive = true;
        OnDeathmatchStarted?.Invoke();
    }
}

