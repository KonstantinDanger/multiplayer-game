using System;

public static class Events
{
    public static Action OnLobbyDisband;
    public static Action OnStartGameInitiated;

    public static void InvokeLobbyDisband()
        => OnLobbyDisband?.Invoke();

    public static void InvokeStartGame()
        => OnStartGameInitiated?.Invoke();
}
