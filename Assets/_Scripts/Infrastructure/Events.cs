using System;

public static class Events
{
    public static Action OnLobbyDisband;
    public static Action OnStartGameInitiated;
    public static Action<Player> OnPlayerAdded;
    public static Action<uint> OnPlayerDemise;

    public static void InvokeLobbyDisband()
        => OnLobbyDisband?.Invoke();

    public static void InvokeStartGame()
        => OnStartGameInitiated?.Invoke();

    public static void InvokePlayerConnected(Player player)
        => OnPlayerAdded?.Invoke(player);

    public static void InvokePlayerDemise(uint playerId)
        => OnPlayerDemise?.Invoke(playerId);
}
