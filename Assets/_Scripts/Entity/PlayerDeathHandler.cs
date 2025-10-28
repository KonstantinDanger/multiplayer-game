using System;

public class PlayerDeathHandler : IPlayerDeathHandler
{
    private readonly Match _match;

    public PlayerDeathHandler(Match match)
        => _match = match;

    public void HandleDeath(Action respawnAction)
    {
        UnityEngine.Debug.Log("is deathmatch active : " + _match.IsDeathmatchActive);

        if (_match.IsDeathmatchActive)
        {
            return;
        }

        UnityEngine.Debug.Log("Respawn ");

        respawnAction();
    }
}
