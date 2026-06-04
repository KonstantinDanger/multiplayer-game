using Mirror;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class MatchStatusReceiver : NetworkBehaviour
{
    public event Action<bool> OnMatchStatusChange;

    [SyncVar(hook = nameof(HandleMatchStatusChange))]
    private bool _isMatchActive = false;

    public bool IsMatchActive => _isMatchActive;

    [Server]
    public void RequestMatchStatusChange(bool active)
        => _isMatchActive = active;

    private void HandleMatchStatusChange(bool oldValue, bool newValue)
        => OnMatchStatusChange?.Invoke(newValue);
}
