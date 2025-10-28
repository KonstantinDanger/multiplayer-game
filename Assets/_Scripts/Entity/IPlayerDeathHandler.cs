using System;

public interface IPlayerDeathHandler
{
    void HandleDeath(Action respawnAction);
}