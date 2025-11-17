using System;

public class PlayerDeathHandler : IPlayerDeathHandler
{
    private readonly Player _player;
    private readonly Match _match;

    private bool _lost;

    public PlayerDeathHandler(Player player, Match match)
    {
        _player = player;
        _match = match;
    }

    public void HandleDeath(Action respawnAction)
    {
        if (_lost)
            return;

        if (_match.IsDeathmatchActive)
        {
            Events.InvokePlayerLost(_player);
            _lost = true;
            return;
        }

        respawnAction();
    }
}
