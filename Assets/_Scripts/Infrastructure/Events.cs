using System;

public static class Events
{
    public static Action ServerOnLobbyDisband;
    public static Action ServerOnStartGameInitiated;
    public static Action<Player> ServerOnPlayerAdded;
    public static Action<uint> ServerOnPlayerDemise;
    public static Action ServerOnHostStop;
    public static Action ServerOnClientDisconnect;

    public static void InvokeLobbyDisband()
        => ServerOnLobbyDisband?.Invoke();

    public static void InvokeStartGame()
        => ServerOnStartGameInitiated?.Invoke();

    public static void InvokePlayerConnected(Player player)
        => ServerOnPlayerAdded?.Invoke(player);

    public static void InvokePlayerDemise(uint playerId)
        => ServerOnPlayerDemise?.Invoke(playerId);

    public static void InvokeHostStop()
        => ServerOnHostStop?.Invoke();

    public static void InvokeClientDisconnect()
        => ServerOnClientDisconnect?.Invoke();
}
