using System;

public static class Events
{
    public static Action OnLobbyDisband;
    public static Action OnStartGameInitiated;
    public static Action<Player> OnPlayerAdded;
    public static Action<Player> OnPlayerLost;

    public static void InvokeLobbyDisband()
        => OnLobbyDisband?.Invoke();

    public static void InvokeStartGame()
        => OnStartGameInitiated?.Invoke();

    public static void InvokePlayerConnected(Player player)
        => OnPlayerAdded?.Invoke(player);

    public static void InvokePlayerLost(Player player)
        => OnPlayerLost?.Invoke(player);
}
